using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Abstraction for detecting duplicate files by content hash.
/// The detector groups files with identical SHA256 hashes and reports
/// the potential space savings.
/// </summary>
public interface IDuplicationDetector
{
    /// <summary>
    /// Detects duplicate files among the given scan results.
    /// Only files that meet the minimum size threshold are hashed.
    /// Files with identical size are grouped, then their SHA256 hashes
    /// are computed to confirm duplicates.
    /// </summary>
    /// <param name="scanResults">The list of scan results to analyze.</param>
    /// <param name="minSizeBytes">The minimum file size in bytes for duplicate detection. Files below this size are skipped.</param>
    /// <param name="cancellationToken">A token to cancel the detection.</param>
    /// <returns>A task that completes when detection finishes, containing all duplicate groups found.</returns>
    Task<IList<DuplicateGroup>> DetectDuplicatesAsync(
        IEnumerable<ScanResult> scanResults,
        long minSizeBytes,
        CancellationToken cancellationToken);
}
