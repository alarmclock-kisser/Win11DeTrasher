using Win11DeTrasher.Models;
using Win11DeTrasher.Services;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="ReportExporter"/> verifying that
/// CSV, JSON, HTML, and TXT exports produce valid output files.
/// </summary>
[TestClass]
public sealed class ReportExporterTests : TestBase
{
    private string _outputDir = string.Empty;
    private ReportExporter _exporter = null!;
    private IList<ScanResult> _testResults = null!;

    [TestInitialize]
    public void Setup()
    {
        this._outputDir = this.CreateTempDirectory();
        this._exporter = new ReportExporter();

        this._testResults = new List<ScanResult>
        {
            new()
            {
                FullPath = @"C:\test\file1.txt",
                Name = "file1.txt",
                Extension = ".txt",
                SizeBytes = 1024,
                SizeDisplay = "1 KB",
                Depth = 1,
                IsDirectory = false,
                LastWriteTime = new DateTime(2024, 1, 15, 10, 30, 0)
            },
            new()
            {
                FullPath = @"C:\test\subdir",
                Name = "subdir",
                Extension = string.Empty,
                SizeBytes = 1048576,
                SizeDisplay = "1 MB",
                Depth = 1,
                IsDirectory = true,
                LastWriteTime = new DateTime(2024, 2, 20, 14, 0, 0),
                FileCount = 5,
                DirectoryCount = 2
            },
            new()
            {
                FullPath = @"C:\test\file2.dll",
                Name = "file2.dll",
                Extension = ".dll",
                SizeBytes = 5368709120,
                SizeDisplay = "5 GB",
                Depth = 1,
                IsDirectory = false,
                LastWriteTime = new DateTime(2023, 6, 10, 8, 15, 0)
            }
        };
    }

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            Directory.Delete(this._outputDir, recursive: true);
        }
        catch
        {
            // Ignore cleanup failures
        }
    }

    [TestMethod]
    public void ExportCsv_ShouldCreateValidFile()
    {
        var path = Path.Combine(this._outputDir, "test.csv");
        var success = this._exporter.ExportCsv(this._testResults, path);

        success.ShouldBeTrue();
        File.Exists(path).ShouldBeTrue();

        var content = File.ReadAllText(path);
        content.ShouldContain("Full Path,Name,Extension");
        content.ShouldContain("file1.txt");
        content.ShouldContain("file2.dll");
        content.ShouldContain("subdir");
    }

    [TestMethod]
    public void ExportCsv_WithCommasInPath_ShouldEscape()
    {
        var results = new List<ScanResult>
        {
            new()
            {
                FullPath = @"C:\path,with\commas\file.txt",
                Name = "file.txt",
                Extension = ".txt",
                SizeBytes = 100,
                SizeDisplay = "100 B",
                Depth = 3,
                IsDirectory = false
            }
        };

        var path = Path.Combine(this._outputDir, "escaped.csv");
        var success = this._exporter.ExportCsv(results, path);

        success.ShouldBeTrue();

        var content = File.ReadAllText(path);
        content.ShouldContain("\"C:\\path,with\\commas\\file.txt\"");
    }

    [TestMethod]
    public void ExportJson_ShouldCreateValidFile()
    {
        var path = Path.Combine(this._outputDir, "test.json");
        var success = this._exporter.ExportJson(this._testResults, path);

        success.ShouldBeTrue();
        File.Exists(path).ShouldBeTrue();

        var content = File.ReadAllText(path);
        content.ShouldContain("file1.txt");
        content.ShouldContain("file2.dll");
        content.ShouldContain("subdir");
        content.ShouldStartWith("[");
    }

    [TestMethod]
    public void ExportHtml_ShouldCreateValidFile()
    {
        var path = Path.Combine(this._outputDir, "test.html");
        var success = this._exporter.ExportHtml(this._testResults, path);

        success.ShouldBeTrue();
        File.Exists(path).ShouldBeTrue();

        var content = File.ReadAllText(path);
        content.ShouldContain("<!DOCTYPE html>");
        content.ShouldContain("Win11DeTrasher Scan Report");
        content.ShouldContain("file1.txt");
        content.ShouldContain("<table>");
    }

    [TestMethod]
    public void ExportTxt_ShouldCreateValidFile()
    {
        var path = Path.Combine(this._outputDir, "test.txt");
        var success = this._exporter.ExportTxt(this._testResults, path);

        success.ShouldBeTrue();
        File.Exists(path).ShouldBeTrue();

        var content = File.ReadAllText(path);
        content.ShouldContain("Win11DeTrasher Scan Report");
        content.ShouldContain("file1.txt");
        content.ShouldContain("[FILE]");
        content.ShouldContain("[DIR ]");
    }

    [TestMethod]
    public void ExportCsv_InvalidPath_ShouldReturnFalse()
    {
        var path = @"C:\invalid\path\that\does\not\exist\file.csv";
        var success = this._exporter.ExportCsv(this._testResults, path);
        success.ShouldBeFalse();
    }

    [TestMethod]
    public void ExportJson_EmptyResults_ShouldCreateValidFile()
    {
        var path = Path.Combine(this._outputDir, "empty.json");
        var success = this._exporter.ExportJson([], path);

        success.ShouldBeTrue();
        File.Exists(path).ShouldBeTrue();

        var content = File.ReadAllText(path);
        content.ShouldBe("[]");
    }
}
