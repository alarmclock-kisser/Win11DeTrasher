using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Abstraction for the concurrent file scanner. The scanner traverses a
/// directory tree using a work queue with a configurable number of worker
/// threads, applies filters, and publishes progress updates via
/// <see cref="IProgressTracker"/>.
/// </summary>
public interface IFileScanner : IDisposable
{
    /// <summary>
    /// Gets or sets the number of worker threads to use for scanning.
    /// Can be changed on-the-fly during a scan: the scanner adapts its
    /// worker pool without aborting the running operation.
    /// Maximum is clamped to <see cref="Environment.ProcessorCount"/>.
    /// </summary>
    int WorkerCount { get; set; }

    /// <summary>
    /// Starts a concurrent scan of the specified root path.
    /// The scan runs on background threads and publishes progress updates
    /// via the <see cref="IProgressTracker"/>.
    /// </summary>
    /// <param name="rootPath">The root directory or drive to scan (e.g. "C:\").</param>
    /// <param name="filterOptions">The filter options to apply during the scan.</param>
    /// <param name="cancellationToken">A token to cancel the scan.</param>
    /// <returns>A task that completes when the scan finishes, containing all discovered <see cref="ScanResult"/> entries.</returns>
    Task<IList<ScanResult>> ScanAsync(
        string rootPath,
        ScanFilterOptions filterOptions,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a value indicating whether a scan is currently running.
    /// </summary>
    bool IsScanning { get; }
}
