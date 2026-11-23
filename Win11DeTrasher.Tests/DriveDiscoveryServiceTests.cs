using Win11DeTrasher.Services;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="DriveDiscoveryService"/>.
/// Verifies that the service returns a valid list of drives
/// and that the drive entries have the expected properties.
/// </summary>
[TestClass]
public sealed class DriveDiscoveryServiceTests : TestBase
{
    [TestMethod]
    public async Task GetAvailableDrivesAsync_ShouldReturnAtLeastOneDrive()
    {
        var service = new DriveDiscoveryService();
        var drives = await service.GetAvailableDrivesAsync();

        // On a Windows system, there should be at least one drive (C:)
        drives.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [TestMethod]
    public async Task GetAvailableDrivesAsync_ShouldIncludeCDrive()
    {
        var service = new DriveDiscoveryService();
        var drives = await service.GetAvailableDrivesAsync();

        var cDrive = drives.FirstOrDefault(d => d.Letter.StartsWith("C", StringComparison.OrdinalIgnoreCase));

        // C: should be present on most Windows systems
        cDrive.ShouldNotBeNull();
        if (cDrive != null)
        {
            cDrive.TotalBytes.ShouldBeGreaterThan(0);
            cDrive.FreeBytes.ShouldBeGreaterThanOrEqualTo(0);
        }
    }

    [TestMethod]
    public async Task GetAvailableDrivesAsync_DriveType_ShouldBeValid()
    {
        var service = new DriveDiscoveryService();
        var drives = await service.GetAvailableDrivesAsync();

        var validTypes = new[] { "Local Disk", "Removable", "Network", "CD-ROM", "RAM Disk", "Unknown" };

        drives.All(d => validTypes.Contains(d.DriveType)).ShouldBeTrue();
    }

    [TestMethod]
    public async Task GetAvailableDrivesAsync_TotalShouldBeGreaterThanFree()
    {
        var service = new DriveDiscoveryService();
        var drives = await service.GetAvailableDrivesAsync();

        drives.Where(d => d.TotalBytes > 0)
            .All(d => d.TotalBytes >= d.FreeBytes)
            .ShouldBeTrue();
    }
}
