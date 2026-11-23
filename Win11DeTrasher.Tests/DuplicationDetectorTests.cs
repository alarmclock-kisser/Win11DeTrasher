using Win11DeTrasher.Models;
using Win11DeTrasher.Services;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="DuplicationDetector"/> using a temporary
/// directory with duplicate files.
/// </summary>
[TestClass]
public sealed class DuplicationDetectorTests : TestBase
{
    private string _root = string.Empty;
    private DuplicationDetector _detector = null!;

    [TestInitialize]
    public void Setup()
    {
        this._root = this.CreateTempDirectory();
        this._detector = new DuplicationDetector();

        // Create files:
        // dup1.txt: "hello world" (11 bytes)
        // dup2.txt: "hello world" (11 bytes) — duplicate of dup1
        // dup3.txt: "hello world" (11 bytes) — duplicate of dup1
        // unique1.txt: "unique content here" (19 bytes)
        // unique2.txt: "another unique file" (19 bytes) — same size as unique1 but different content
        this.CreateFile(this._root, "dup1.txt", "hello world");
        this.CreateFile(this._root, "dup2.txt", "hello world");
        this.CreateFile(this._root, "dup3.txt", "hello world");
        this.CreateFile(this._root, "unique1.txt", "unique content here");
        this.CreateFile(this._root, "unique2.txt", "another unique file");
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

    private IList<ScanResult> BuildScanResults()
    {
        var results = new List<ScanResult>();

        foreach (var file in Directory.EnumerateFiles(this._root))
        {
            var fileInfo = new FileInfo(file);
            results.Add(new ScanResult
            {
                FullPath = file,
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                SizeBytes = fileInfo.Length,
                Depth = 1,
                IsDirectory = false
            });
        }

        return results;
    }

    [TestMethod]
    public async Task DetectDuplicatesAsync_ShouldFindDuplicateGroup()
    {
        var results = this.BuildScanResults();

        var groups = await this._detector.DetectDuplicatesAsync(
            results,
            minSizeBytes: 0, // Include all files
            CancellationToken.None);

        // Should find one group: dup1, dup2, dup3 (all "hello world")
        groups.Count.ShouldBe(1);

        var group = groups[0];
        group.FileCount.ShouldBe(3);
        group.PotentialSavingsBytes.ShouldBe(11 * 2); // 2 extra copies of 11 bytes
        group.Files.Count.ShouldBe(3);
    }

    [TestMethod]
    public async Task DetectDuplicatesAsync_SameSizeDifferentContent_ShouldNotGroup()
    {
        var results = this.BuildScanResults();

        var groups = await this._detector.DetectDuplicatesAsync(
            results,
            minSizeBytes: 0,
            CancellationToken.None);

        // unique1.txt and unique2.txt have the same size (19 bytes) but different content
        // They should NOT be grouped together
        var groupWithUnique = groups.FirstOrDefault(g => g.Files.Any(f => f.Contains("unique1.txt")));
        groupWithUnique.ShouldBeNull();
    }

    [TestMethod]
    public async Task DetectDuplicatesAsync_MinSize_ShouldExcludeSmallFiles()
    {
        var results = this.BuildScanResults();

        // Set min size to 12 bytes — all our files are 11 or 19 bytes
        // "hello world" is 11 bytes, so dup files are excluded
        var groups = await this._detector.DetectDuplicatesAsync(
            results,
            minSizeBytes: 12,
            CancellationToken.None);

        // unique1 and unique2 are 19 bytes but have different content → no duplicates
        groups.Count.ShouldBe(0);
    }

    [TestMethod]
    public async Task DetectDuplicatesAsync_NoDuplicates_ShouldReturnEmpty()
    {
        // Create a directory with only unique files
        var uniqueRoot = this.CreateTempDirectory();
        this.CreateFile(uniqueRoot, "a.txt", "alpha");
        this.CreateFile(uniqueRoot, "b.txt", "bravo");
        this.CreateFile(uniqueRoot, "c.txt", "charlie");

        var results = this.BuildScanResultsFor(uniqueRoot);

        var groups = await this._detector.DetectDuplicatesAsync(
            results,
            minSizeBytes: 0,
            CancellationToken.None);

        groups.Count.ShouldBe(0);

        Directory.Delete(uniqueRoot, recursive: true);
    }

    [TestMethod]
    public async Task DetectDuplicatesAsync_Cancellation_ShouldThrow()
    {
        var results = this.BuildScanResults();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            async () => await this._detector.DetectDuplicatesAsync(
                results,
                minSizeBytes: 0,
                cts.Token));
    }

    private IList<ScanResult> BuildScanResultsFor(string root)
    {
        var results = new List<ScanResult>();

        foreach (var file in Directory.EnumerateFiles(root))
        {
            var fileInfo = new FileInfo(file);
            results.Add(new ScanResult
            {
                FullPath = file,
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                SizeBytes = fileInfo.Length,
                Depth = 1,
                IsDirectory = false
            });
        }

        return results;
    }
}
