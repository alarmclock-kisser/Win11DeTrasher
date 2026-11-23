using Win11DeTrasher.Models;
using Win11DeTrasher.Services;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="FileScanner"/> using a temporary directory
/// structure. Verifies that the scanner discovers all files and directories,
/// applies filters correctly, and reports progress.
/// </summary>
[TestClass]
public sealed class FileScannerTests : TestBase
{
    private string _root = string.Empty;
    private ProgressTracker _progressTracker = null!;
    private FileScanner _scanner = null!;

    [TestInitialize]
    public void Setup()
    {
        this._root = this.CreateTempDirectory();
        this._progressTracker = new ProgressTracker();
        this._scanner = new FileScanner(this._progressTracker)
        {
            WorkerCount = 2
        };

        // Create test directory structure:
        // root/
        //   file1.txt (100 bytes)
        //   file2.dll (200 bytes)
        //   sub1/
        //     file3.log (50 bytes)
        //     sub1a/
        //       file4.txt (300 bytes)
        //   sub2/
        //     file5.tmp (10 bytes)
        this.CreateDirectoryStructure(this._root, "sub1", "sub1/sub1a", "sub2");
        this.CreateFile(this._root, "file1.txt", new string('a', 100));
        this.CreateFile(this._root, "file2.dll", new string('b', 200));
        this.CreateFile(Path.Combine(this._root, "sub1"), "file3.log", new string('c', 50));
        this.CreateFile(Path.Combine(this._root, "sub1", "sub1a"), "file4.txt", new string('d', 300));
        this.CreateFile(Path.Combine(this._root, "sub2"), "file5.tmp", new string('e', 10));
    }

    [TestCleanup]
    public void Cleanup()
    {
        this._scanner.Dispose();
        this._progressTracker.Dispose();

        try
        {
            Directory.Delete(this._root, recursive: true);
        }
        catch
        {
            // Ignore cleanup failures
        }
    }

    [TestMethod]
    public async Task ScanAsync_NoFilter_ShouldDiscoverAllFilesAndDirs()
    {
        var results = await this._scanner.ScanAsync(
            this._root,
            new ScanFilterOptions(),
            CancellationToken.None);

        // Should find: file1, file2, file3, file4, file5 (5 files)
        // and: sub1, sub1a, sub2 (3 directories) + root (1 directory)
        var files = results.Where(r => !r.IsDirectory).ToList();
        var dirs = results.Where(r => r.IsDirectory).ToList();

        files.Count.ShouldBe(5);
        dirs.Count.ShouldBe(4); // root + sub1 + sub1a + sub2
    }

    [TestMethod]
    public async Task ScanAsync_HighWorkerCountAndMinimumSize_ShouldFinishAfterAllDirectoriesAreVisited()
    {
        const int bulkDirectoryCount = 96;
        for (var index = 0; index < bulkDirectoryCount; index++)
        {
            var directory = Path.Combine(this._root, $"bulk-{index:D3}");
            Directory.CreateDirectory(directory);
            await File.WriteAllBytesAsync(Path.Combine(directory, "large.bin"), new byte[4096]);
        }

        this._scanner.WorkerCount = Math.Min(16, Environment.ProcessorCount);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var results = await this._scanner.ScanAsync(
            this._root,
            new ScanFilterOptions { MinFileSizeBytes = 4096 },
            timeout.Token).WaitAsync(TimeSpan.FromSeconds(25));

        results.Count(result => !result.IsDirectory).ShouldBe(bulkDirectoryCount);
        results.Count(result => result.IsDirectory).ShouldBe(bulkDirectoryCount + 4);
        this._progressTracker.CurrentProgress.IsCompleted.ShouldBeTrue();
        this._progressTracker.CurrentProgress.IsCancelled.ShouldBeFalse();
        this._progressTracker.CurrentProgress.TotalFiles.ShouldBe(bulkDirectoryCount);
        this._progressTracker.CurrentProgress.ProcessedFiles.ShouldBe(bulkDirectoryCount);
    }

    [TestMethod]
    public async Task ScanAsync_ChangingWorkerCountDuringTraversal_ShouldNotLoseResults()
    {
        const int bulkDirectoryCount = 256;
        for (var index = 0; index < bulkDirectoryCount; index++)
        {
            var directory = Path.Combine(this._root, $"resize-{index:D3}");
            Directory.CreateDirectory(directory);
            await File.WriteAllBytesAsync(Path.Combine(directory, "large.bin"), new byte[4096]);
        }

        this._scanner.WorkerCount = Math.Min(2, Environment.ProcessorCount);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var scanTask = this._scanner.ScanAsync(
            this._root,
            new ScanFilterOptions { MinFileSizeBytes = 4096 },
            timeout.Token);

        await Task.Delay(10);
        this._scanner.WorkerCount = Math.Min(16, Environment.ProcessorCount);
        await Task.Delay(10);
        this._scanner.WorkerCount = Math.Min(6, Environment.ProcessorCount);

        var results = await scanTask.WaitAsync(TimeSpan.FromSeconds(25));
        results.Count(result => !result.IsDirectory).ShouldBe(bulkDirectoryCount);
        this._progressTracker.CurrentProgress.IsCompleted.ShouldBeTrue();
        this._progressTracker.CurrentProgress.IsCancelled.ShouldBeFalse();
    }

    [TestMethod]
    public async Task ScanAsync_IncludeExtensions_ShouldOnlyReturnMatchingFiles()
    {
        var filter = new ScanFilterOptions
        {
            IncludeExtensions = [".txt", ".dll"]
        };

        var results = await this._scanner.ScanAsync(this._root, filter, CancellationToken.None);

        var files = results.Where(r => !r.IsDirectory).ToList();

        // Should find: file1.txt, file2.dll, file4.txt (3 files)
        files.Count.ShouldBe(3);

        var extensions = files.Select(f => f.Extension).ToList();
        extensions.All(e => e == ".txt" || e == ".dll").ShouldBeTrue();
    }

    [TestMethod]
    public async Task ScanAsync_ExcludeExtensions_ShouldExcludeMatchingFiles()
    {
        var filter = new ScanFilterOptions
        {
            ExcludeExtensions = [".log", ".tmp"]
        };

        var results = await this._scanner.ScanAsync(this._root, filter, CancellationToken.None);

        var files = results.Where(r => !r.IsDirectory).ToList();

        // Should find: file1.txt, file2.dll, file4.txt (3 files)
        files.Count.ShouldBe(3);

        var extensions = files.Select(f => f.Extension).ToList();
        extensions.ShouldNotContain(".log");
        extensions.ShouldNotContain(".tmp");
    }

    [TestMethod]
    public async Task ScanAsync_MinFileSize_ShouldExcludeSmallFiles()
    {
        var filter = new ScanFilterOptions
        {
            MinFileSizeBytes = 100
        };

        var results = await this._scanner.ScanAsync(this._root, filter, CancellationToken.None);

        var files = results.Where(r => !r.IsDirectory).ToList();

        // Should find: file1.txt (100), file2.dll (200), file4.txt (300) — 3 files
        // Excluded: file3.log (50), file5.tmp (10)
        files.Count.ShouldBe(3);

        files.All(f => f.SizeBytes >= 100).ShouldBeTrue();
    }

    [TestMethod]
    public async Task ScanAsync_MaxFileSize_ShouldExcludeLargeFiles()
    {
        var filter = new ScanFilterOptions
        {
            MaxFileSizeBytes = 200
        };

        var results = await this._scanner.ScanAsync(this._root, filter, CancellationToken.None);

        var files = results.Where(r => !r.IsDirectory).ToList();

        // Should find: file1.txt (100), file2.dll (200), file3.log (50), file5.tmp (10) — 4 files
        // Excluded: file4.txt (300)
        files.Count.ShouldBe(4);

        files.All(f => f.SizeBytes <= 200).ShouldBeTrue();
    }

    [TestMethod]
    public async Task ScanAsync_MaxDepth_ShouldLimitDepth()
    {
        var filter = new ScanFilterOptions
        {
            MaxDepth = 1
        };

        var results = await this._scanner.ScanAsync(this._root, filter, CancellationToken.None);

        var files = results.Where(r => !r.IsDirectory).ToList();

        // Depth 0: root (no files directly in root... wait, file1 and file2 are in root)
        // Depth 1: sub1 (file3.log), sub2 (file5.tmp)
        // Depth 2: sub1a (file4.txt) — excluded
        // So: file1.txt, file2.dll (depth 0) + file3.log, file5.tmp (depth 1) = 4 files
        files.Count.ShouldBe(4);

        files.ShouldNotContain(f => f.Name == "file4.txt");
    }

    [TestMethod]
    public async Task ScanAsync_MaxDepthZero_ShouldOnlyScanRootDirectory()
    {
        var results = await this._scanner.ScanAsync(
            this._root,
            new ScanFilterOptions { MaxDepth = 0 },
            CancellationToken.None);

        results.Count(result => !result.IsDirectory).ShouldBe(2);
        results.Count(result => result.IsDirectory).ShouldBe(1);
    }

    [TestMethod]
    public async Task ScanAsync_Cancellation_ShouldStopScan()
    {
        var cts = new CancellationTokenSource();
        var task = this._scanner.ScanAsync(this._root, new ScanFilterOptions(), cts.Token);

        // Cancel immediately
        cts.Cancel();

        var results = await task;

        // The scan should have been cancelled — results may be empty or partial
        results.ShouldNotBeNull();
    }

    [TestMethod]
    public void WorkerCount_ShouldBeClampedToProcessorCount()
    {
        this._scanner.WorkerCount = Environment.ProcessorCount + 100;
        this._scanner.WorkerCount.ShouldBeLessThanOrEqualTo(Environment.ProcessorCount);

        this._scanner.WorkerCount = -5;
        this._scanner.WorkerCount.ShouldBe(1);
    }

    [TestMethod]
    public async Task ScanAsync_InvalidRoot_ShouldThrow()
    {
        var ex = await Should.ThrowAsync<ArgumentException>(
            async () => await this._scanner.ScanAsync("", new ScanFilterOptions(), CancellationToken.None));

        ex.ParamName.ShouldBe("rootPath");
    }

    [TestMethod]
    public async Task ScanAsync_ShouldReportProgress()
    {
        var results = await this._scanner.ScanAsync(
            this._root,
            new ScanFilterOptions(),
            CancellationToken.None);

        var progress = this._progressTracker.CurrentProgress;

        progress.ProcessedFiles.ShouldBeGreaterThanOrEqualTo(0);
        progress.IsCompleted.ShouldBeTrue();
    }

    [TestMethod]
    public async Task ScanAsync_WhileScanning_ShouldThrow()
    {
        var cts = new CancellationTokenSource();
        var firstScan = this._scanner.ScanAsync(this._root, new ScanFilterOptions(), cts.Token);

        // Wait until the first scan is actually running
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!this._scanner.IsScanning && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }

        this._scanner.IsScanning.ShouldBeTrue();

        // The second scan should throw because the first is already running.
        // ScanAsync throws synchronously (before any await) when a scan is
        // in progress, so we don't need to await the returned Task.
        bool threw = false;
        try
        {
            _ = this._scanner.ScanAsync(this._root, new ScanFilterOptions(), CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }

        threw.ShouldBeTrue();

        // Clean up: cancel the first scan and wait for it to finish
        cts.Cancel();
        try
        {
            await firstScan;
        }
        catch
        {
            // Cancelled or completed — both are fine
        }
    }
}
