namespace Win11DeTrasher.Models;

/// <summary>
/// The outcome of a cleanup operation (move to Recycle Bin or hard delete)
/// for a single file or directory.
/// </summary>
/// <param name="FullPath">The full path of the affected file or directory.</param>
/// <param name="Success">Gets a value indicating whether the operation succeeded.</param>
/// <param name="ErrorMessage">The error message when the operation failed, or null on success.</param>
/// <param name="FreedBytes">The number of bytes freed by this operation (0 on failure).</param>
public sealed record CleanupResult(
    string FullPath,
    bool Success,
    string? ErrorMessage,
    long FreedBytes);

/// <summary>
/// Aggregated results of a batch cleanup operation across multiple files or directories.
/// </summary>
/// <param name="TotalItems">The total number of items processed.</param>
/// <param name="Succeeded">The number of items successfully processed.</param>
/// <param name="Failed">The number of items that failed.</param>
/// <param name="TotalFreedBytes">The total number of bytes freed.</param>
/// <param name="Results">The individual results for each item.</param>
public sealed record BatchCleanupResult(
    int TotalItems,
    int Succeeded,
    int Failed,
    long TotalFreedBytes,
    List<CleanupResult> Results);
