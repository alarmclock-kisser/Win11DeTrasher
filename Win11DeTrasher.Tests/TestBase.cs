namespace Win11DeTrasher.Tests;

/// <summary>
/// Base class for all tests. Provides common test utilities.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Gets the test context.
    /// </summary>
    public TestContext TestContext { get; set; } = null!;

    /// <summary>
    /// Creates a temporary directory for test use, which is cleaned up
    /// after the test completes.
    /// </summary>
    /// <returns>The full path to the temporary directory.</returns>
    protected string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"Win11DeTrasherTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Creates a file with the specified content in the given directory.
    /// </summary>
    /// <param name="directory">The directory to create the file in.</param>
    /// <param name="fileName">The name of the file to create.</param>
    /// <param name="content">The content to write to the file.</param>
    /// <returns>The full path to the created file.</returns>
    protected string CreateFile(string directory, string fileName, string content)
    {
        var path = Path.Combine(directory, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    /// <summary>
    /// Creates a file with the specified size (in bytes) using random data.
    /// </summary>
    /// <param name="directory">The directory to create the file in.</param>
    /// <param name="fileName">The name of the file to create.</param>
    /// <param name="sizeBytes">The size of the file in bytes.</param>
    /// <returns>The full path to the created file.</returns>
    protected string CreateFileWithSize(string directory, string fileName, int sizeBytes)
    {
        var path = Path.Combine(directory, fileName);
        var data = new byte[sizeBytes];
        var random = new Random(42); // Fixed seed for reproducibility
        random.NextBytes(data);
        File.WriteAllBytes(path, data);
        return path;
    }

    /// <summary>
    /// Creates a nested directory structure for testing.
    /// </summary>
    /// <param name="root">The root directory.</param>
    /// <param name="structure">A list of relative paths to create (e.g. ["sub1", "sub1/file1.txt", "sub2"]).</param>
    protected void CreateDirectoryStructure(string root, params string[] structure)
    {
        foreach (var path in structure)
        {
            var fullPath = Path.Combine(root, path);
            if (Path.GetExtension(path) != string.Empty)
            {
                // It's a file — create the directory and an empty file
                var dir = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(fullPath, string.Empty);
            }
            else
            {
                Directory.CreateDirectory(fullPath);
            }
        }
    }
}
