using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// A single drive entry for display in the UI.
/// </summary>
/// <param name="Letter">The drive letter (e.g. "C").</param>
/// <param name="Name">The drive label or volume name (e.g. "Windows"), or an empty string when not set.</param>
/// <param name="DriveType">A human-readable drive type description (e.g. "Local Disk", "Removable", "Network").</param>
/// <param name="TotalBytes">The total size in bytes, or 0 when not available.</param>
/// <param name="FreeBytes">The free space in bytes, or 0 when not available.</param>
public sealed record DriveEntry(
    string Letter,
    string Name,
    string DriveType,
    long TotalBytes,
    long FreeBytes);

/// <summary>
/// Abstraction for discovering available drives on the system, used by the UI
/// for the drive ComboBox and by tests for mocking.
/// </summary>
public interface IDriveDiscoveryService
{
    /// <summary>
    /// Gets a list of all available drives on the system.
    /// </summary>
    /// <returns>A list of <see cref="DriveEntry"/> instances, or an empty list when no drives are found.</returns>
    Task<IList<DriveEntry>> GetAvailableDrivesAsync();
}
