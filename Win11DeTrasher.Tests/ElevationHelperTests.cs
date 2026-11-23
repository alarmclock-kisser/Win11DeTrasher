using Win11DeTrasher.Utils;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="ElevationHelper.RequiresElevation"/>.
/// Tests the protected-path detection logic.
/// </summary>
[TestClass]
public sealed class ElevationHelperTests : TestBase
{
    [TestMethod]
    public void RequiresElevation_WindowsPath_ShouldReturnTrue()
    {
        ElevationHelper.RequiresElevation(@"C:\Windows").ShouldBeTrue();
        ElevationHelper.RequiresElevation(@"C:\Windows\System32").ShouldBeTrue();
        ElevationHelper.RequiresElevation(@"C:\Windows\Temp").ShouldBeTrue();
    }

    [TestMethod]
    public void RequiresElevation_ProgramFiles_ShouldReturnTrue()
    {
        ElevationHelper.RequiresElevation(@"C:\Program Files").ShouldBeTrue();
        ElevationHelper.RequiresElevation(@"C:\Program Files\App1").ShouldBeTrue();
    }

    [TestMethod]
    public void RequiresElevation_ProgramFilesX86_ShouldReturnTrue()
    {
        ElevationHelper.RequiresElevation(@"C:\Program Files (x86)").ShouldBeTrue();
        ElevationHelper.RequiresElevation(@"C:\Program Files (x86)\App2").ShouldBeTrue();
    }

    [TestMethod]
    public void RequiresElevation_UserPath_ShouldReturnFalse()
    {
        ElevationHelper.RequiresElevation(@"C:\Users\testuser").ShouldBeFalse();
        ElevationHelper.RequiresElevation(@"C:\Users\testuser\source\repos").ShouldBeFalse();
    }

    [TestMethod]
    public void RequiresElevation_OtherDrive_ShouldReturnFalse()
    {
        ElevationHelper.RequiresElevation(@"D:\").ShouldBeFalse();
        ElevationHelper.RequiresElevation(@"D:\Windows").ShouldBeFalse();
    }

    [TestMethod]
    public void RequiresElevation_EmptyPath_ShouldReturnFalse()
    {
        ElevationHelper.RequiresElevation(string.Empty).ShouldBeFalse();
        ElevationHelper.RequiresElevation(null!).ShouldBeFalse();
    }

    [TestMethod]
    public void RequiresElevation_CaseInsensitive_ShouldMatch()
    {
        ElevationHelper.RequiresElevation(@"C:\WINDOWS").ShouldBeTrue();
        ElevationHelper.RequiresElevation(@"c:\windows").ShouldBeTrue();
        ElevationHelper.RequiresElevation(@"C:\Program FILES").ShouldBeTrue();
    }

    [TestMethod]
    public void RequiresElevation_NoFalsePositiveOnSimilarPrefix()
    {
        // "C:\Windows.old" should NOT match "C:\Windows"
        ElevationHelper.RequiresElevation(@"C:\Windows.old").ShouldBeFalse();
    }

    [TestMethod]
    public void RequiresElevation_TrailingBackslash_ShouldHandle()
    {
        ElevationHelper.RequiresElevation(@"C:\Windows\").ShouldBeTrue();
        ElevationHelper.RequiresElevation(@"C:\Users\testuser\").ShouldBeFalse();
    }
}
