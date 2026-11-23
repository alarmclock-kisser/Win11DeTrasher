using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Thread-safe progress tracking for a scan operation.
/// The scanner publishes progress updates; the UI subscribes to the
/// <see cref="ProgressUpdated"/> event to refresh the progress bar,
/// elapsed time, ETA, and file counts in real time.
/// </summary>
public interface IProgressTracker : IDisposable
{
    /// <summary>
    /// Event raised when the scan progress has been updated.
    /// The event is raised on the thread that called <see cref="Update"/>.
    /// </summary>
    event Action<ScanProgress>? ProgressUpdated;

    /// <summary>
    /// Gets the most recent progress snapshot.
    /// </summary>
    ScanProgress CurrentProgress { get; }

    /// <summary>
    /// Gets a value indicating whether the tracker has been started for a scan.
    /// </summary>
    bool IsStarted { get; }

    /// <summary>
    /// Starts a new progress tracking session, resetting all counters.
    /// </summary>
    void Start();

    /// <summary>
    /// Records a batch of discovered files with their sizes.
    /// Updates the total bytes and total file count.
    /// </summary>
    /// <param name="fileCount">The number of files discovered in this batch.</param>
    /// <param name="totalBytes">The total size in bytes of the files in this batch.</param>
    void ReportDiscovered(int fileCount, long totalBytes);

    /// <summary>
    /// Records a batch of processed files with their sizes.
    /// Updates the processed bytes and processed file count.
    /// </summary>
    /// <param name="fileCount">The number of files processed in this batch.</param>
    /// <param name="totalBytes">The total size in bytes of the files in this batch.</param>
    void ReportProcessed(int fileCount, long totalBytes);

    /// <summary>
    /// Reports an error (file that could not be accessed).
    /// </summary>
    void ReportError();

    /// <summary>
    /// Marks the scan as completed.
    /// </summary>
    void Complete();

    /// <summary>
    /// Marks the scan as cancelled.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Resets the tracker to its initial state (not started).
    /// </summary>
    void Reset();
}
