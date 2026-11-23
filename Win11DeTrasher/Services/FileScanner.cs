using System.Collections.Concurrent;
using System.IO;
using Win11DeTrasher.Shared;
using Win11DeTrasher.Models;
using Win11DeTrasher.Utils;

namespace Win11DeTrasher.Services;

/// <summary>
/// Concurrent implementation of <see cref="IFileScanner"/>.
/// Uses a <see cref="BlockingCollection{T}"/> work queue with a dynamic worker
/// pool. The number of active workers can be adjusted on-the-fly by changing
/// <see cref="WorkerCount"/> while a scan is running — the scanner adapts
/// by starting or retiring worker tasks accordingly, without aborting the scan.
/// Completion is signaled via a pending-directory counter: when all enqueued
/// directories have been fully processed (all subdirectories discovered and
/// enqueued), the counter reaches zero and <c>CompleteAdding</c> is called,
/// allowing workers to drain the queue and exit.
/// </summary>
public class FileScanner : IFileScanner, IDisposable
{
    private readonly IProgressTracker _progressTracker;
    private readonly IRollingFileMemoryLogger? _logger;
    private readonly object _sync = new();

    private BlockingCollection<string>? _workQueue;
    private List<Task>? _workerTasks;
    private CancellationTokenSource? _cts;
    private List<ScanResult>? _results;
    private int _activeWorkerCount;
    private int _targetWorkerCount;
    private bool _isScanning;

    // Scan context (set before each scan)
    private string _rootPath = string.Empty;
    private ScanFilterOptions? _filterOptions;

    // Pending directory counter: tracks how many directories are enqueued but
    // not yet fully processed. When it reaches 0, all directories have been
    // processed (all subdirectories discovered and enqueued), and CompleteAdding
    // can be called safely.
    private int _pendingDirectoryCount;
    private bool _isCompleted;
    private bool _isFinalizing;
    private int _scanErrorCount;

    public FileScanner(IProgressTracker progressTracker, IRollingFileMemoryLogger? logger = null)
    {
        this._progressTracker = progressTracker ?? throw new ArgumentNullException(nameof(progressTracker));
        this._logger = logger;
        this._targetWorkerCount = Math.Max(1, Math.Min(4, Environment.ProcessorCount));
    }

    /// <inheritdoc/>
    public int WorkerCount
    {
        get
        {
            lock (this._sync)
            {
                return this._targetWorkerCount;
            }
        }
        set
        {
            var clamped = Math.Max(1, Math.Min(value, Environment.ProcessorCount));

            lock (this._sync)
            {
                this._targetWorkerCount = clamped;

                // If a scan is running, adapt the worker pool on-the-fly
                if (this._isScanning
                    && !this._isCompleted
                    && !this._isFinalizing
                    && this._cts is { IsCancellationRequested: false }
                    && this._workerTasks != null)
                {
                    this.AdaptWorkerPool(clamped);
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool IsScanning
    {
        get
        {
            lock (this._sync)
            {
                return this._isScanning;
            }
        }
    }

    /// <inheritdoc/>
    public Task<IList<ScanResult>> ScanAsync(
        string rootPath,
        ScanFilterOptions filterOptions,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Root path must not be null or empty.", nameof(rootPath));
        }

        filterOptions ??= new ScanFilterOptions();

        lock (this._sync)
        {
            if (this._isScanning)
            {
                throw new InvalidOperationException("A scan is already in progress.");
            }

            // Set scan context
            this._rootPath = Path.GetFullPath(rootPath);
            this._filterOptions = filterOptions;

            // Initialize the work queue
            this._workQueue = new BlockingCollection<string>(this._targetWorkerCount * 4);
            this._results = [];
            this._cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this._isScanning = true;
            this._activeWorkerCount = 0;
            this._pendingDirectoryCount = 1;
            this._isCompleted = false;
            this._isFinalizing = false;
            this._scanErrorCount = 0;

            this._progressTracker.Start();

            // Start initial worker pool
            this._workerTasks = [];
            for (int i = 0; i < this._targetWorkerCount; i++)
            {
                this.StartWorker();
            }

        }

        // The root is counted before it becomes visible to workers. Incrementing
        // after Add races a fast worker, which could decrement the counter first
        // and make the scan appear complete before traversal has started.
        try
        {
            this._workQueue!.Add(this._rootPath, this._cts!.Token);
        }
        catch (OperationCanceledException)
        {
            this.MarkQueueCompleteAfterRootEnqueueFailure();
        }
        catch (Exception ex)
        {
            this._logger?.Log(ex, preText: $"Could not enqueue scan root '{this._rootPath}'");
            this._progressTracker.ReportError();
            this.MarkQueueCompleteAfterRootEnqueueFailure();
        }

        return this.AwaitCompletionAsync();
    }

    private async Task<IList<ScanResult>> AwaitCompletionAsync()
    {
        var isCancelled = false;

        try
        {
            // Wait for all worker tasks to complete.
            // Workers exit when the queue is empty and CompleteAdding has been called,
            // or when the cancellation token fires.
            // CompleteAdding is triggered by the pending-directory counter reaching
            // zero (see OnDirectoryProcessed) — this happens AFTER all directories
            // have been fully processed (all subdirectories enqueued), so no
            // subdirectory will be lost.
            while (true)
            {
                Task[] workerTasks;
                lock (this._sync)
                {
                    workerTasks = this._workerTasks?.ToArray() ?? [];
                }

                await Task.WhenAll(workerTasks);

                lock (this._sync)
                {
                    if (this._workerTasks != null && this._workerTasks.Count > workerTasks.Length)
                    {
                        continue;
                    }

                    // Stop accepting worker-count changes before deciding that
                    // the complete worker-task set has finished.
                    this._isFinalizing = true;
                    break;
                }
            }

            isCancelled = this._cts?.IsCancellationRequested == true;
            if (!isCancelled && !this._isCompleted)
            {
                this._logger?.LogError(
                    $"Worker pool stopped with {Volatile.Read(ref this._pendingDirectoryCount):N0} directory item(s) pending; marking the scan incomplete.");
                isCancelled = true;
            }
            if (isCancelled)
            {
                this._progressTracker.Cancel();
            }
        }
        catch (OperationCanceledException)
        {
            isCancelled = true;
            this._progressTracker.Cancel();
        }
        catch (Exception ex)
        {
            this._logger?.Log(ex, preText: $"Worker pool failed while scanning '{this._rootPath}'");
            isCancelled = true;
            this._progressTracker.Cancel();
        }
        finally
        {
            // Safety net: if the pending counter didn't reach 0 (e.g. due to an
            // error), force CompleteAdding so workers can exit and the testhost
            // doesn't crash from lingering threads.
            this.EnsureQueueCompleted();
            this.CleanupAfterScan();
        }

        if (!isCancelled)
        {
            this._progressTracker.Complete();
            this._logger?.LogSuccess($"Scan workers completed. Traversal errors: {Volatile.Read(ref this._scanErrorCount):N0}.");
        }
        else
        {
            this._logger?.LogWarning($"Scan stopped. Partial traversal errors: {Volatile.Read(ref this._scanErrorCount):N0}.");
        }

        return this._results!;
    }

    private void MarkQueueCompleteAfterRootEnqueueFailure()
    {
        Interlocked.Exchange(ref this._pendingDirectoryCount, 0);
        Interlocked.Exchange(ref this._isCompleted, true);
        this.EnsureQueueCompleted();
    }

    /// <summary>
    /// Called when a directory has been fully processed (all its subdirectories
    /// have been discovered and enqueued). Decrements the pending directory counter.
    /// When the counter reaches zero, signals the work queue that no more items
    /// will be added, allowing workers to drain the queue and exit.
    /// </summary>
    private void OnDirectoryProcessed()
    {
        var remaining = Interlocked.Decrement(ref this._pendingDirectoryCount);

        if (remaining == 0 && !this._isCompleted)
        {
            Interlocked.Exchange(ref this._isCompleted, true);
            try
            {
                this._workQueue?.CompleteAdding();
            }
            catch (Exception)
            {
                // Already completed or disposed
            }
        }
    }

    /// <summary>
    /// Ensures the work queue's CompleteAdding has been called.
    /// Called to guarantee workers can exit, preventing testhost crashes.
    /// </summary>
    private void EnsureQueueCompleted()
    {
        var queue = this._workQueue;
        if (queue == null)
        {
            return;
        }

        try
        {
            queue.CompleteAdding();
        }
        catch (Exception)
        {
            // Already completed or disposed — either way, no action needed
        }
    }

    /// <summary>
    /// Cleans up resources after a scan completes or is cancelled.
    /// Disposes the work queue and resets state. Does NOT call
    /// CompleteAdding() — that is done in AwaitCompletionAsync before
    /// waiting for workers to finish.
    /// </summary>
    private void CleanupAfterScan()
    {
        lock (this._sync)
        {
            this._isScanning = false;
            this._isFinalizing = false;

            this._workQueue?.Dispose();
            this._workQueue = null;
            this._workerTasks = null;
            this._activeWorkerCount = 0;
            // Intentionally do not dispose the CTS — workers may still be
            // registered on it. The CTS will be garbage collected.
        }
    }

    /// <summary>
    /// Starts a new worker task that processes items from the work queue.
    /// Must be called under the _sync lock.
    /// </summary>
    private void StartWorker()
    {
        var cts = this._cts!;
        var task = Task.Run(() => this.WorkerLoop(cts.Token));
        this._workerTasks!.Add(task);
        this._activeWorkerCount++;
    }

    /// <summary>
    /// The main loop for a single worker thread.
    /// Dequeues paths from the work queue, processes them, and enqueues
    /// subdirectory paths for further processing.
    /// Results are batched locally and flushed to the shared list periodically
    /// to minimize lock contention.
    /// </summary>
    private void WorkerLoop(CancellationToken token)
    {
        var localResults = new List<ScanResult>();
        var localDiscoveredFiles = 0;
        var localDiscoveredBytes = 0L;
        var localProcessedFiles = 0;
        var localProcessedBytes = 0L;
        var localErrors = 0;
        var flushCounter = 0;
        var workerRegistered = true;
        var localDirectories = new Stack<string>();

        try
        {
            while (true)
            {
                if (localDirectories.Count == 0 && this.TryRetireWorker())
                {
                    workerRegistered = false;
                    return;
                }

                var queue = this._workQueue;
                if (queue == null)
                {
                    return;
                }

                string? path;
                if (localDirectories.Count > 0)
                {
                    path = localDirectories.Pop();
                }
                else if (!queue.TryTake(out path, 100, token))
                {
                    if (queue.IsCompleted)
                    {
                        return;
                    }

                    continue;
                }

                if (path is null)
                {
                    continue;
                }

                token.ThrowIfCancellationRequested();

                try
                {
                    this.ProcessPath(path, token, localDirectories, localResults,
                        ref localDiscoveredFiles, ref localDiscoveredBytes,
                        ref localProcessedFiles, ref localProcessedBytes,
                        ref localErrors);
                }
                catch (Exception ex)
                {
                    localErrors++;
                    this.RecordScanError(path, ex);
                }

                flushCounter++;
                if (flushCounter >= 500)
                {
                    this.FlushResults(localResults, localDiscoveredFiles, localDiscoveredBytes,
                        localProcessedFiles, localProcessedBytes, localErrors);
                    localResults.Clear();
                    localDiscoveredFiles = 0;
                    localDiscoveredBytes = 0;
                    localProcessedFiles = 0;
                    localProcessedBytes = 0;
                    localErrors = 0;
                    flushCounter = 0;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Clean exit on cancellation
        }
        catch (Exception ex)
        {
            localErrors++;
            this.RecordScanError(this._rootPath, ex);
        }
        finally
        {
            // If an unexpected queue failure exits this worker, release pending
            // directory slots for any local overflow work it did not process.
            while (localDirectories.Count > 0)
            {
                localDirectories.Pop();
                this.OnDirectoryProcessed();
            }

            this.FlushResults(localResults, localDiscoveredFiles, localDiscoveredBytes,
                localProcessedFiles, localProcessedBytes, localErrors);

            if (workerRegistered)
            {
                lock (this._sync)
                {
                    this._activeWorkerCount = Math.Max(0, this._activeWorkerCount - 1);
                }
            }
        }
    }

    private bool TryRetireWorker()
    {
        lock (this._sync)
        {
            if (this._isScanning && this._activeWorkerCount > this._targetWorkerCount)
            {
                this._activeWorkerCount--;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Processes a single file or directory path, applying the configured filters.
    /// Appends the result to the local batch list.
    /// Enqueues subdirectory paths for further processing by any worker.
    /// Tracks pending directory count to signal completion.
    /// </summary>
    private void ProcessPath(
        string path,
        CancellationToken token,
        Stack<string> localDirectories,
        List<ScanResult> localResults,
        ref int localDiscoveredFiles,
        ref long localDiscoveredBytes,
        ref int localProcessedFiles,
        ref long localProcessedBytes,
        ref int localErrors)
    {
        try
        {
            // The work queue contains directories only (the root and discovered
            // subdirectories). Always decrement its pending count in finally,
            // including when processing throws or the directory is filtered.
            var result = this.TryProcessPath(path, localResults, ref localErrors);
            if (result == null)
            {
                return;
            }

            localResults.Add(result);
            localProcessedBytes += result.SizeBytes;

            if (result.IsDirectory)
            {
                // Directory processing inspects each eligible file directly;
                // reflect those files in progress instead of reporting only
                // directory work items (which previously left Files at 0/0).
                localDiscoveredFiles += result.FileCount;
                localProcessedFiles += result.FileCount;
                localDiscoveredBytes += result.SizeBytes;
            }
            else
            {
                localDiscoveredFiles++;
                localProcessedFiles++;
                localDiscoveredBytes += result.SizeBytes;
            }

            foreach (var subDir in result.SubdirectoryPaths)
            {
                // Reserve the pending slot before publishing the directory to
                // another worker; otherwise that worker may finish it first.
                Interlocked.Increment(ref this._pendingDirectoryCount);
                try
                {
                    if (!this._workQueue!.TryAdd(subDir, 0, token))
                    {
                        localDirectories.Push(subDir);
                    }
                }
                catch (InvalidOperationException)
                {
                    this.OnDirectoryProcessed();
                    break;
                }
                catch
                {
                    this.OnDirectoryProcessed();
                    throw;
                }
            }
        }
        finally
        {
            this.OnDirectoryProcessed();
        }
    }

    /// <summary>
    /// Tries to process a single file or directory path, applying the configured filters.
    /// Returns a <see cref="ScanResult"/> for the path, or null when the path
    /// should be skipped (filtered out, inaccessible, etc.).
    /// File results from directory processing are added to the provided list.
    /// </summary>
    private ScanResult? TryProcessPath(string path, List<ScanResult> fileResults, ref int localErrors)
    {
        try
        {
            var attrs = File.GetAttributes(path);

            // Check for hidden/system skip options
            var filterOptions = this._filterOptions;
            var isSelectedRoot = this.IsSelectedRoot(path);
            if (filterOptions is { SkipHidden: true }
                && (attrs & FileAttributes.Hidden) != 0
                && !isSelectedRoot)
            {
                return null;
            }

            if (filterOptions is { SkipSystem: true }
                && (attrs & FileAttributes.System) != 0
                && !isSelectedRoot)
            {
                return null;
            }

            if (isSelectedRoot && (attrs & (FileAttributes.Hidden | FileAttributes.System)) != 0)
            {
                this._logger?.LogInfo(
                    $"The selected scan root '{path}' is marked Hidden/System; scanning the explicitly selected root anyway.");
            }

            var isDirectory = (attrs & FileAttributes.Directory) != 0;

            if (isDirectory)
            {
                return this.TryProcessDirectory(path, filterOptions, fileResults, ref localErrors);
            }

            return this.TryProcessFile(path, filterOptions, ref localErrors);
        }
        catch (Exception ex)
        {
            localErrors++;
            this.RecordScanError(path, ex);
            return null;
        }
    }

    private bool IsSelectedRoot(string path)
    {
        return string.Equals(
            Path.TrimEndingDirectorySeparator(path),
            Path.TrimEndingDirectorySeparator(this._rootPath),
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Processes a directory entry: collects subdirectory paths and file results,
    /// applying depth and filter limits. File results are added to the provided
    /// list so they appear in the scan output alongside the directory entry.
    /// </summary>
    private ScanResult? TryProcessDirectory(
        string path,
        ScanFilterOptions? filterOptions,
        List<ScanResult> fileResults,
        ref int localErrors)
    {
        var depth = this.CalculateDepth(path);

        // Check depth limit — if this directory exceeds the limit, skip it entirely
        if (filterOptions is { MaxDepth: >= 0 } && depth > filterOptions.MaxDepth)
        {
            return null;
        }

        var result = new ScanResult
        {
            FullPath = path,
            Name = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar)),
            Extension = string.Empty,
            SizeBytes = 0,
            Depth = depth,
            IsDirectory = true,
            LastWriteTime = this.GetLastWriteTimeSafe(path),
            FileCount = 0,
            DirectoryCount = 0,
            SubdirectoryPaths = []
        };

        try
        {
            var subDirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            result.DirectoryCount = subDirs.Length;

            // Collect subdirectory paths for enqueueing
            foreach (var subDir in subDirs)
            {
                var subDepth = depth + 1;
                if (filterOptions is { MaxDepth: >= 0 } && subDepth > filterOptions.MaxDepth)
                {
                    continue;
                }

                result.SubdirectoryPaths.Add(subDir);
            }

            // Process files directly in this directory
            foreach (var file in files)
            {
                var fileResult = this.TryProcessFile(file, filterOptions, ref localErrors);
                if (fileResult != null)
                {
                    result.SizeBytes += fileResult.SizeBytes;
                    result.FileCount++;
                    fileResults.Add(fileResult);
                }
            }
        }
        catch (Exception ex)
        {
            localErrors++;
            this.RecordScanError(path, ex);
            // Still report the directory entry with its available metadata.
        }

        return result;
    }

    /// <summary>
    /// Processes a file entry, applying all configured filters
    /// (size, extension, path segments). Returns null when filtered out.
    /// </summary>
    private ScanResult? TryProcessFile(
        string path,
        ScanFilterOptions? filterOptions,
        ref int localErrors)
    {
        if (filterOptions == null)
        {
            return null;
        }

        try
        {
            var attributes = File.GetAttributes(path);
            if (filterOptions.SkipHidden && (attributes & FileAttributes.Hidden) != 0)
            {
                return null;
            }

            if (filterOptions.SkipSystem && (attributes & FileAttributes.System) != 0)
            {
                return null;
            }

            var fileInfo = new FileInfo(path);

            // Check min/max size filters
            if (filterOptions.MinFileSizeBytes > 0 && fileInfo.Length < filterOptions.MinFileSizeBytes)
            {
                return null;
            }

            if (filterOptions.MaxFileSizeBytes < long.MaxValue && fileInfo.Length > filterOptions.MaxFileSizeBytes)
            {
                return null;
            }

            var extension = fileInfo.Extension;

            // Check extension include filter
            if (filterOptions.IncludeExtensions.Count > 0 &&
                !filterOptions.IncludeExtensions.Any(e => string.Equals(e, extension, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            // Check extension exclude filter
            if (filterOptions.ExcludeExtensions.Count > 0 &&
                filterOptions.ExcludeExtensions.Any(e => string.Equals(e, extension, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            // Check path segment filters
            var relativePath = this.GetRelativePath(path);
            if (filterOptions.IncludePathSegments.Count > 0 &&
                !filterOptions.IncludePathSegments.Any(s => relativePath.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            if (filterOptions.ExcludePathSegments.Count > 0 &&
                filterOptions.ExcludePathSegments.Any(s => relativePath.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            return new ScanResult
            {
                FullPath = path,
                Name = fileInfo.Name,
                Extension = extension,
                SizeBytes = fileInfo.Length,
                Depth = this.CalculateDepth(path),
                IsDirectory = false,
                LastWriteTime = this.GetLastWriteTimeSafe(path),
                FileCount = 0,
                DirectoryCount = 0,
                SizeDisplay = SizeFormatter.Format(fileInfo.Length)
            };
        }
        catch (Exception ex)
        {
            localErrors++;
            this.RecordScanError(path, ex);
            return null;
        }
    }

    private void RecordScanError(string path, Exception exception)
    {
        var errorNumber = Interlocked.Increment(ref this._scanErrorCount);

        // Access-denied and IO failures are common when traversing the Windows
        // volume. Record a small sample plus periodic summaries, not one line per
        // inaccessible file. Unexpected exceptions retain their full details.
        if (errorNumber <= 10 || errorNumber % 1000 == 0)
        {
            if (exception is UnauthorizedAccessException or IOException)
            {
                this._logger?.LogWarning($"Skipped '{path}' ({exception.GetType().Name}: {exception.Message}); scan errors so far: {errorNumber:N0}.");
            }
            else
            {
                this._logger?.Log(exception, preText: $"Unexpected scan failure at '{path}' (error {errorNumber:N0})");
            }
        }
    }

    /// <summary>
    /// Calculates the depth of a path relative to the scan root.
    /// The root itself has depth 0; immediate children have depth 1, etc.
    /// </summary>
    private int CalculateDepth(string path)
    {
        if (string.IsNullOrWhiteSpace(this._rootPath))
        {
            return 0;
        }

        var root = this._rootPath.TrimEnd(Path.DirectorySeparatorChar);
        if (path.TrimEnd(Path.DirectorySeparatorChar).Equals(root, StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        var relative = path[root.Length..].TrimStart(Path.DirectorySeparatorChar);
        if (string.IsNullOrEmpty(relative))
        {
            return 0;
        }

        return relative.Count(c => c == Path.DirectorySeparatorChar) + 1;
    }

    /// <summary>
    /// Gets the relative path of a file or directory, lowercased for
    /// case-insensitive filter matching.
    /// </summary>
    private string GetRelativePath(string path)
    {
        if (string.IsNullOrWhiteSpace(this._rootPath))
        {
            return path.ToLowerInvariant();
        }

        var root = this._rootPath.TrimEnd(Path.DirectorySeparatorChar);
        if (path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            return path[root.Length..].TrimStart(Path.DirectorySeparatorChar).ToLowerInvariant();
        }

        return path.ToLowerInvariant();
    }

    private DateTime GetLastWriteTimeSafe(string path)
    {
        try
        {
            return File.GetLastWriteTime(path);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Flushes a batch of results from a worker thread to the shared result list
    /// and reports progress.
    /// </summary>
    private void FlushResults(
        List<ScanResult> results,
        int discoveredFiles,
        long discoveredBytes,
        int processedFiles,
        long processedBytes,
        int errors)
    {
        if (results.Count > 0)
        {
            lock (this._sync)
            {
                this._results?.AddRange(results);
            }
        }

        this._progressTracker.ReportDiscovered(discoveredFiles, discoveredBytes);
        this._progressTracker.ReportProcessed(processedFiles, processedBytes);

        for (int i = 0; i < errors; i++)
        {
            this._progressTracker.ReportError();
        }
    }

    /// <summary>
    /// Adapts the worker pool to the new target count while a scan is running.
    /// Starts workers on increases; workers retire cooperatively on decreases.
    /// </summary>
    private void AdaptWorkerPool(int newTarget)
    {
        // Must be called under the _sync lock
        var currentCount = this._activeWorkerCount;

        if (newTarget > currentCount)
        {
            for (int i = currentCount; i < newTarget; i++)
            {
                this.StartWorker();
            }
        }
        // Decreasing the target is observed by workers in their short queue-poll
        // loop; they retire without dropping any directories.
    }

    public void Dispose()
    {
        BlockingCollection<string>? queueToDispose;
        Task[]? tasksToWait;

        lock (this._sync)
        {
            // Cancel any in-flight scan so workers exit promptly.
            // Do NOT dispose the CTS here — worker threads may still be
            // registered on it, and disposing an in-use CTS can cause
            // a native crash. The CTS will be garbage collected.
            if (this._isScanning && this._cts != null)
            {
                this._cts.Cancel();
            }

            queueToDispose = this._workQueue;
            tasksToWait = this._workerTasks?.ToArray();

            this._isScanning = false;
            this._workerTasks = null;
            this._workQueue = null;
            // Intentionally do not null out _cts or dispose it.
        }

        // Wait for workers to finish (they will exit once the queue is
        // completed and cancelled). Use a timeout to avoid blocking forever.
        if (tasksToWait is { Length: > 0 })
        {
            try
            {
                Task.WaitAll(tasksToWait, 3000);
            }
            catch
            {
                // Some workers may not have finished — that's acceptable
            }
        }

        try
        {
            queueToDispose?.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Already disposed
        }
    }
}
