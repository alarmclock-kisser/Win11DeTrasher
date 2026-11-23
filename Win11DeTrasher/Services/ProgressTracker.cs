using System.Diagnostics;
using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Thread-safe implementation of <see cref="IProgressTracker"/>.
/// All state mutations are guarded by a <see cref="lock"/>; progress events
/// are throttled to avoid flooding the UI thread with updates.
/// </summary>
public class ProgressTracker : IProgressTracker, IDisposable
{
    private readonly object _sync = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly Random _random = new();

    private ScanProgress _currentProgress = new(
        TotalBytes: 0,
        ProcessedBytes: 0,
        TotalFiles: 0,
        ProcessedFiles: 0,
        ElapsedSeconds: 0,
        EstimatedRemainingSeconds: -1,
        ProgressPercent: 0,
        IsCompleted: false,
        IsCancelled: false,
        ErrorCount: 0);

    private bool _isStarted;
    private DateTime _lastEventTime = DateTime.MinValue;
    private const double MinEventIntervalSeconds = 0.2;

    /// <inheritdoc/>
    public event Action<ScanProgress>? ProgressUpdated;

    /// <inheritdoc/>
    public ScanProgress CurrentProgress
    {
        get
        {
            lock (this._sync)
            {
                return this._currentProgress;
            }
        }
    }

    /// <inheritdoc/>
    public bool IsStarted
    {
        get
        {
            lock (this._sync)
            {
                return this._isStarted;
            }
        }
    }

    /// <inheritdoc/>
    public void Start()
    {
        lock (this._sync)
        {
            this._currentProgress = new ScanProgress(0, 0, 0, 0, 0, -1, 0, false, false, 0);
            this._isStarted = true;
            this._stopwatch.Restart();
        }
    }

    /// <inheritdoc/>
    public void ReportDiscovered(int fileCount, long totalBytes)
    {
        ScanProgress snapshot;
        bool shouldRaise;

        lock (this._sync)
        {
            if (!this._isStarted)
            {
                return;
            }

            this._currentProgress = this._currentProgress with
            {
                TotalBytes = this._currentProgress.TotalBytes + totalBytes,
                TotalFiles = this._currentProgress.TotalFiles + fileCount
            };

            // Update the progress percent (total changed, so percent changes)
            this._currentProgress = this.UpdateProgressPercent(
                this._currentProgress,
                this._currentProgress.ProcessedBytes,
                this._currentProgress.ProcessedFiles);

            snapshot = this.BuildSnapshot();
            shouldRaise = this.ShouldRaiseEvent();
        }

        if (shouldRaise && this.ProgressUpdated != null)
        {
            this.ProgressUpdated(snapshot);
        }
    }

    /// <inheritdoc/>
    public void ReportProcessed(int fileCount, long totalBytes)
    {
        ScanProgress snapshot;
        bool shouldRaise;

        lock (this._sync)
        {
            if (!this._isStarted)
            {
                return;
            }

            var newProcessedBytes = this._currentProgress.ProcessedBytes + totalBytes;
            var newProcessedFiles = this._currentProgress.ProcessedFiles + fileCount;

            this._currentProgress = this._currentProgress with
            {
                ProcessedBytes = newProcessedBytes,
                ProcessedFiles = newProcessedFiles
            };

            // Update the progress percent in the current progress state
            this._currentProgress = this.UpdateProgressPercent(this._currentProgress, newProcessedBytes, newProcessedFiles);

            snapshot = this.BuildSnapshot();
            shouldRaise = this.ShouldRaiseEvent();
        }

        if (shouldRaise && this.ProgressUpdated != null)
        {
            this.ProgressUpdated(snapshot);
        }
    }

    /// <inheritdoc/>
    public void ReportError()
    {
        lock (this._sync)
        {
            if (!this._isStarted)
            {
                return;
            }

            this._currentProgress = this._currentProgress with
            {
                ErrorCount = this._currentProgress.ErrorCount + 1
            };
        }
    }

    /// <inheritdoc/>
    public void Complete()
    {
        ScanProgress snapshot;

        lock (this._sync)
        {
            this._currentProgress = this._currentProgress with
            {
                IsCompleted = true,
                ProcessedBytes = this._currentProgress.TotalBytes,
                ProcessedFiles = this._currentProgress.TotalFiles,
                ProgressPercent = 100,
                EstimatedRemainingSeconds = 0
            };

            snapshot = this.BuildSnapshot();
        }

        this.ProgressUpdated?.Invoke(snapshot);
    }

    /// <inheritdoc/>
    public void Cancel()
    {
        ScanProgress snapshot;

        lock (this._sync)
        {
            this._currentProgress = this._currentProgress with { IsCancelled = true };
            snapshot = this.BuildSnapshot();
        }

        this.ProgressUpdated?.Invoke(snapshot);
    }

    /// <inheritdoc/>
    public void Reset()
    {
        lock (this._sync)
        {
            this._currentProgress = new ScanProgress(0, 0, 0, 0, 0, -1, 0, false, false, 0);
            this._isStarted = false;
            this._stopwatch.Reset();
        }
    }

    private ScanProgress BuildSnapshot()
    {
        var elapsed = this._stopwatch.Elapsed.TotalSeconds;
        var totalBytes = this._currentProgress.TotalBytes;
        var processedBytes = this._currentProgress.ProcessedBytes;
        var totalFiles = this._currentProgress.TotalFiles;
        var processedFiles = this._currentProgress.ProcessedFiles;

        double progressPercent = totalBytes > 0
            ? Math.Min(100.0, (double)processedBytes / totalBytes * 100.0)
            : (totalFiles > 0 ? (double)processedFiles / totalFiles * 100.0 : 0.0);

        double eta = -1;
        if (elapsed > 1.0 && processedBytes > 0 && processedBytes < totalBytes)
        {
            var rate = processedBytes / elapsed;
            if (rate > 0)
            {
                eta = (totalBytes - processedBytes) / rate;
            }
        }

        return this._currentProgress with
        {
            ElapsedSeconds = elapsed,
            EstimatedRemainingSeconds = eta,
            ProgressPercent = progressPercent
        };
    }

    /// <summary>
    /// Recalculates the progress percent and ETA based on the current
    /// processed/total values, and returns an updated <see cref="ScanProgress"/>.
    /// </summary>
    private ScanProgress UpdateProgressPercent(ScanProgress progress, long processedBytes, long processedFiles)
    {
        var elapsed = this._stopwatch.Elapsed.TotalSeconds;
        var totalBytes = progress.TotalBytes;
        var totalFiles = progress.TotalFiles;

        double percent = totalBytes > 0
            ? Math.Min(100.0, (double)processedBytes / totalBytes * 100.0)
            : (totalFiles > 0 ? (double)processedFiles / totalFiles * 100.0 : 0.0);

        double eta = -1;
        if (elapsed > 1.0 && processedBytes > 0 && processedBytes < totalBytes)
        {
            var rate = processedBytes / elapsed;
            if (rate > 0)
            {
                eta = (totalBytes - processedBytes) / rate;
            }
        }

        return progress with
        {
            ElapsedSeconds = elapsed,
            EstimatedRemainingSeconds = eta,
            ProgressPercent = percent
        };
    }

    private bool ShouldRaiseEvent()
    {
        var now = DateTime.UtcNow;
        var elapsed = (now - this._lastEventTime).TotalSeconds;

        // Always raise on completion or cancellation
        if (this._currentProgress.IsCompleted || this._currentProgress.IsCancelled)
        {
            this._lastEventTime = now;
            return true;
        }

        // Throttle: only raise if enough time has passed since last event
        if (elapsed >= MinEventIntervalSeconds)
        {
            this._lastEventTime = now;
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        this._stopwatch.Stop();
    }
}
