using Win11DeTrasher.Services;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="CleanupService"/>.
/// Tests the <see cref="CleanupService.GetTotalSize"/> method and
/// verifies that the service handles various edge cases correctly.
/// Note: Recycle Bin and Hard Delete operations are not tested here
/// because they would actually delete files; they are covered by
/// integration tests on a real system.
/// </summary>
[TestClass]
public sealed class CleanupServiceTests : TestBase
{
    private string _root = string.Empty;
    private CleanupService _cleanupService = null!;

    [TestInitialize]
    public void Setup()
    {
        this._root = this.CreateTempDirectory();
        this._cleanupService = new CleanupService();
    }

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            Directory.Delete(this._root, recursive: true);
        }
        catch
        {
            // Ignore cleanup failures
        }
    }

    [TestMethod]
    public void GetTotalSize_SingleFile_ShouldReturnFileSize()
    {
        var filePath = this.CreateFileWithSize(this._root, "test.bin", 1024);
        var size = this._cleanupService.GetTotalSize([filePath]);

        size.ShouldBe(1024);
    }

    [TestMethod]
    public void GetTotalSize_MultipleFiles_ShouldReturnSumOfSizes()
    {
        var file1 = this.CreateFileWithSize(this._root, "a.bin", 512);
        var file2 = this.CreateFileWithSize(this._root, "b.bin", 1024);
        var file3 = this.CreateFileWithSize(this._root, "c.bin", 2048);

        var size = this._cleanupService.GetTotalSize([file1, file2, file3]);

        size.ShouldBe(512 + 1024 + 2048);
    }

    [TestMethod]
    public void GetTotalSize_Directory_ShouldReturnSumOfAllFiles()
    {
        var subDir = Path.Combine(this._root, "sub");
        Directory.CreateDirectory(subDir);

        this.CreateFileWithSize(this._root, "root.bin", 256);
        this.CreateFileWithSize(subDir, "nested1.bin", 512);
        this.CreateFileWithSize(subDir, "nested2.bin", 1024);

        var size = this._cleanupService.GetTotalSize([this._root]);

        size.ShouldBe(256 + 512 + 1024);
    }

    [TestMethod]
    public void GetTotalSize_NonExistentPath_ShouldReturnZero()
    {
        var nonExistent = Path.Combine(this._root, "does_not_exist.txt");
        var size = this._cleanupService.GetTotalSize([nonExistent]);

        size.ShouldBe(0);
    }

    [TestMethod]
    public void GetTotalSize_EmptyList_ShouldReturnZero()
    {
        var size = this._cleanupService.GetTotalSize([]);
        size.ShouldBe(0);
    }

    [TestMethod]
    public void GetTotalSize_EmptyFile_ShouldReturnZero()
    {
        var filePath = this.CreateFile(this._root, "empty.txt", string.Empty);
        var size = this._cleanupService.GetTotalSize([filePath]);

        size.ShouldBe(0);
    }
}
