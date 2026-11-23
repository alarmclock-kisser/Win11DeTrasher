using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Abstraction for cleanup operations: moving files/directories to the
/// Recycle Bin or performing hard delete (permanent removal).
/// </summary>
public interface ICleanupService
{
    /// <summary>
    /// Moves the specified files and directories to the Windows Recycle Bin.
    /// This operation is reversible — items can be restored from the Recycle Bin.
    /// </summary>
    /// <param name="paths">The list of file or directory paths to send to the Recycle Bin.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the operation finishes, containing the batch results.</returns>
    Task<BatchCleanupResult> SendToRecycleBinAsync(
        IList<string> paths,
        CancellationToken cancellationToken);

    /// <summary>
    /// Permanently deletes the specified files and directories (hard delete).
    /// This operation is NOT reversible. The caller should ensure the user
    /// has confirmed the deletion before calling this method.
    /// </summary>
    /// <param name="paths">The list of file or directory paths to permanently delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the operation finishes, containing the batch results.</returns>
    Task<BatchCleanupResult> HardDeleteAsync(
        IList<string> paths,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the total size in bytes of the given paths (for display in confirmation dialogs).
    /// </summary>
    /// <param name="paths">The list of file or directory paths to measure.</param>
    /// <returns>The total size in bytes.</returns>
    long GetTotalSize(IList<string> paths);
}
