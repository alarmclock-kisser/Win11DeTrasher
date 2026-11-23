namespace Win11DeTrasher.Models;

/// <summary>
/// Configuration options for a scan, including filters, depth limits, and
/// duplicate detection settings. Serializable to JSON for preset storage.
/// </summary>
public class ScanFilterOptions
{
    /// <summary>Gets or sets the maximum scan depth relative to the root. A value of 0 means only the root level is scanned. Use -1 for unlimited depth (default).</summary>
    public int MaxDepth { get; set; } = -1;

    /// <summary>Gets or sets a value indicating whether to include subdirectories in the results. When false, only files directly in each directory are reported.</summary>
    public bool IncludeSubdirectories { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of file extensions to include (e.g. [".dll", ".onnx"]).
    /// When empty or null, all extensions are included. Matching is case-insensitive.
    /// </summary>
    public List<string> IncludeExtensions { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of file extensions to exclude (e.g. [".tmp", ".log"]).
    /// Matching is case-insensitive.
    /// </summary>
    public List<string> ExcludeExtensions { get; set; } = [];

    /// <summary>
    /// Gets or sets a list of relative path segments to include.
    /// When non-empty, only files whose relative path contains at least one of these segments are reported.
    /// Matching is case-insensitive.
    /// </summary>
    public List<string> IncludePathSegments { get; set; } = [];

    /// <summary>
    /// Gets or sets a list of relative path segments to exclude.
    /// When non-empty, files whose relative path contains any of these segments are skipped.
    /// Matching is case-insensitive.
    /// </summary>
    public List<string> ExcludePathSegments { get; set; } = [];

    /// <summary>
    /// Gets or sets the minimum file size in bytes for a file to be included in results.
    /// Files smaller than this threshold are excluded. Default: 0 (include all).
    /// </summary>
    public long MinFileSizeBytes { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum file size in bytes for a file to be included in results.
    /// Files larger than this threshold are excluded. Default: long.MaxValue (no limit).
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = long.MaxValue;

    /// <summary>Gets or sets a value indicating whether to skip hidden files and directories (Windows hidden attribute).</summary>
    public bool SkipHidden { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to skip system files and directories (Windows system attribute).</summary>
    public bool SkipSystem { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether to skip files in use (locked files).</summary>
    public bool SkipLockedFiles { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to detect duplicates by SHA256 hash.</summary>
    public bool DetectDuplicates { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum file size in bytes for duplicate detection.
    /// Only files at or above this size are hashed when duplicate detection is enabled.
    /// Default: 1 MB (1048576 bytes).
    /// </summary>
    public long DuplicateMinSizeBytes { get; set; } = 1048576;
}
