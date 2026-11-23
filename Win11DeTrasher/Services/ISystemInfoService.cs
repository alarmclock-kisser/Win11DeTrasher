using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Abstraction for collecting system information relevant for
/// disk space analysis: temp folder sizes, Recycle Bin size,
/// large file lists, and file age statistics.
/// </summary>
public interface ISystemInfoService
{
    /// <summary>
    /// Gets a list of system info entries for the current system,
    /// including temp folders, Recycle Bin, and other space-hungry locations.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when collection finishes, containing the system info entries.</returns>
    Task<IList<SystemInfoEntry>> GetSystemInfoAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a list of the largest files on the specified root path,
    /// sorted by size descending.
    /// </summary>
    /// <param name="rootPath">The root path to scan (e.g. "C:\").</param>
    /// <param name="topN">The maximum number of entries to return.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the operation finishes, containing the top N largest files.</returns>
    Task<IList<ScanResult>> GetLargestFilesAsync(
        string rootPath,
        int topN,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets statistics about file age distribution for the specified root path.
    /// Returns counts of files in age buckets (0-30 days, 30-90 days, 90-365 days, 1-5 years, 5+ years).
    /// </summary>
    /// <param name="rootPath">The root path to scan (e.g. "C:\").</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the operation finishes, containing the age statistics.</returns>
    Task<Dictionary<string, int>> GetFileAgeStatsAsync(
        string rootPath,
        CancellationToken cancellationToken);
}
