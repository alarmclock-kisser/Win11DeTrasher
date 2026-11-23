using System.IO;
using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Default implementation of <see cref="IDriveDiscoveryService"/>.
/// Enumerates available drives using the Win32 API (via <see cref="DriveInfo"/>)
/// and returns them as a list of <see cref="DriveEntry"/> records.
/// </summary>
public class DriveDiscoveryService : IDriveDiscoveryService
{
    /// <summary>
    /// Gets a list of all available drives on the system.
    /// </summary>
    /// <returns>A list of <see cref="DriveEntry"/> instances.</returns>
    public async Task<IList<DriveEntry>> GetAvailableDrivesAsync()
    {
        // Offload to a background thread to avoid blocking the UI
        var drives = await Task.Run(() =>
        {
            var results = new List<DriveEntry>();

            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    // Skip drives that are not ready (e.g. empty CD-ROM, disconnected USB)
                    if (!drive.IsReady)
                    {
                        continue;
                    }

                    string driveType = drive.DriveType switch
                    {
                        System.IO.DriveType.Fixed => "Local Disk",
                        System.IO.DriveType.Removable => "Removable",
                        System.IO.DriveType.Network => "Network",
                        System.IO.DriveType.CDRom => "CD-ROM",
                        System.IO.DriveType.Ram => "RAM Disk",
                        _ => "Unknown"
                    };

                    results.Add(new DriveEntry(
                        drive.Name.TrimEnd('\\'),
                        drive.VolumeLabel,
                        driveType,
                        drive.TotalSize,
                        drive.AvailableFreeSpace));
                }
                catch
                {
                    // Skip drives that throw on property access (e.g. network drives that are offline)
                }
            }

            return results;
        });

        return drives;
    }
}
