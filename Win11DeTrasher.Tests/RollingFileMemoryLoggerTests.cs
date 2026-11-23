using Win11DeTrasher.Shared;

namespace Win11DeTrasher.Tests;

[TestClass]
public sealed class RollingFileMemoryLoggerTests : TestBase
{
    [TestMethod]
    public void Constructor_WithMaxLogFiles16_KeepsAtMost16IncludingCurrentFile()
    {
        var logDirectory = this.CreateTempDirectory();

        try
        {
            for (var index = 0; index < 20; index++)
            {
                var path = Path.Combine(logDirectory, $"log_20200101_0000{index:D2}.txt");
                File.WriteAllText(path, "previous session");
                File.SetCreationTime(path, DateTime.Now.AddDays(-20 + index));
            }

            _ = new RollingFileMemoryLogger(new RollingFileMemoryLoggerOptions
            {
                LogDirectory = logDirectory,
                CreateLogFile = true,
                MaxLogFiles = 16
            });

            Directory.GetFiles(logDirectory, "log_*.txt").Length.ShouldBe(16);
        }
        finally
        {
            Directory.Delete(logDirectory, recursive: true);
        }
    }
}
