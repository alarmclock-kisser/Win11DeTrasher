using Win11DeTrasher.Utils;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="SizeFormatter"/>.
/// </summary>
[TestClass]
public sealed class SizeFormatterTests : TestBase
{
    [TestMethod]
    [DataRow(0, "0 B")]
    [DataRow(1023, "1023 B")]
    [DataRow(1024, "1 KB")]
    [DataRow(1536, "1.5 KB")]
    [DataRow(1048576, "1 MB")]
    [DataRow(1073741824, "1 GB")]
    [DataRow(1099511627776L, "1 TB")]
    [DataRow(-1024, "-1 KB")]
    public void Format_ShouldReturnCorrectString(long bytes, string expected)
    {
        var result = SizeFormatter.Format(bytes);
        result.ShouldBe(expected);
    }

    [TestMethod]
    [DataRow("100 B", 100L)]
    [DataRow("1 KB", 1024L)]
    [DataRow("1.5 MB", 1572864L)]
    [DataRow("2 GB", 2147483648L)]
    [DataRow("0", 0L)]
    public void TryParse_ValidInput_ShouldParseCorrectly(string input, long expectedBytes)
    {
        var success = SizeFormatter.TryParse(input, out var bytes);
        success.ShouldBeTrue();
        bytes.ShouldBe(expectedBytes);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("abc")]
    [DataRow("100 XB")]
    [DataRow("-5 KB")]
    public void TryParse_InvalidInput_ShouldReturnFalse(string input)
    {
        var success = SizeFormatter.TryParse(input, out _);
        success.ShouldBeFalse();
    }
}
