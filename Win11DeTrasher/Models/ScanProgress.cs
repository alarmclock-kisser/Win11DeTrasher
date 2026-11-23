namespace Win11DeTrasher.Models;

/// <summary>
/// A snapshot of the current scan progress, suitable for thread-safe publication
/// to the UI thread. All values are in bytes or seconds.
/// </summary>
/// <param name="TotalBytes">Total bytes discovered so far (for progress calculation).</param>
/// <param name="ProcessedBytes">Total bytes processed so far (for progress calculation).</param>
/// <param name="TotalFiles">Total files discovered so far.</param>
/// <param name="ProcessedFiles">Total files processed so far.</param>
/// <param name="ElapsedSeconds">Elapsed time in seconds since scan start.</param>
/// <param name="EstimatedRemainingSeconds">Estimated seconds remaining, or -1 when not yet computable.</param>
/// <param name="ProgressPercent">Progress as a percentage (0–100), or 0 when not yet computable.</param>
/// <param name="IsCompleted">Gets a value indicating whether the scan has completed.</param>
/// <param name="IsCancelled">Gets a value indicating whether the scan was cancelled.</param>
/// <param name="ErrorCount">Number of files that could not be accessed (permission denied, in use, etc.).</param>
public readonly record struct ScanProgress(
    long TotalBytes,
    long ProcessedBytes,
    int TotalFiles,
    int ProcessedFiles,
    double ElapsedSeconds,
    double EstimatedRemainingSeconds,
    double ProgressPercent,
    bool IsCompleted,
    bool IsCancelled,
    int ErrorCount);
