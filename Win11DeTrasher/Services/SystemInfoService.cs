using System.IO;
using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Default implementation of <see cref="ISystemInfoService"/>.
/// Collects system information by querying well-known Windows locations
/// and analyzing file metadata.
/// </summary>
public class SystemInfoService : ISystemInfoService
{
    /// <inheritdoc/>
    public async Task<IList<SystemInfoEntry>> GetSystemInfoAsync(CancellationToken cancellationToken)
    {
        var entries = new List<SystemInfoEntry>();

        // Offload to a background thread
        await Task.Run(() =>
        {
            // Windows Temp folder
            var winTempPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
            entries.Add(this.GetDirectoryInfo("Windows Temp", winTempPath, "System-wide temporary files"));

            // User Temp folder
            var userTempPath = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar);
            entries.Add(this.GetDirectoryInfo("User Temp", userTempPath, "Current user's temporary files"));

            // Prefetch
            var prefetchPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
            entries.Add(this.GetDirectoryInfo("Prefetch", prefetchPath, "Application launch optimization cache"));

            // Recycle Bin
            var recycleBinSize = this.GetRecycleBinSize();
            entries.Add(new SystemInfoEntry(
                "Recycle Bin",
                string.Empty,
                recycleBinSize,
                "Deleted files waiting for permanent removal"));

            // WinSxS (component store)
            var winsxsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows), "WinSxS");
            entries.Add(this.GetDirectoryInfo("WinSxS", winsxsPath, "Windows component store (use DISM to clean)"));

            // Delivery Optimization
            var deliveryOptPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows), "ServiceProfiles");
            entries.Add(this.GetDirectoryInfo("Service Profiles", deliveryOptPath, "Windows service profiles"));

            // Large files in ProgramData
            var programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            entries.Add(this.GetDirectoryInfo("ProgramData", programDataPath, "System-wide application data"));
        }, cancellationToken);

        return entries;
    }

    private SystemInfoEntry GetDirectoryInfo(string category, string path, string description)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            return new SystemInfoEntry(category, path, 0, description);
        }

        long totalSize = 0;
        try
        {
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    totalSize += new FileInfo(file).Length;
                }
                catch
                {
                    // Skip inaccessible files
                }
            }
        }
        catch
        {
            // Inaccessible directory
        }

        return new SystemInfoEntry(category, path, totalSize, description);
    }

    private long GetRecycleBinSize()
    {
        // Use SHQueryRecycleBin via P/Invoke
        try
        {
            var info = new SHQUERYINFO
            {
                cbSize = System.Runtime.InteropServices.Marshal.SizeOf<SHQUERYINFO>()
            };

            int hr = SHQueryRecycleBin(null, ref info);
            return hr == 0 ? info.i64Size : 0;
        }
        catch
        {
            return 0;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct SHQUERYINFO
    {
        public int cbSize;
        public long i64Size;
        public long i64NumItems;
    }

    [System.Runtime.InteropServices.DllImport("shell32.dll", SetLastError = true)]
    private static extern int SHQueryRecycleBin(string? pszDrive, ref SHQUERYINFO pqri);

    /// <inheritdoc/>
    public async Task<IList<ScanResult>> GetLargestFilesAsync(
        string rootPath,
        int topN,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Root path must not be null or empty.", nameof(rootPath));
        }

        return await Task.Run(() =>
        {
            var results = new List<ScanResult>();

            try
            {
                foreach (var file in Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var fileInfo = new FileInfo(file);
                        results.Add(new ScanResult
                        {
                            FullPath = file,
                            Name = fileInfo.Name,
                            Extension = fileInfo.Extension,
                            SizeBytes = fileInfo.Length,
                            Depth = 0,
                            IsDirectory = false,
                            LastWriteTime = this.GetLastWriteTimeSafe(file),
                            SizeDisplay = Utils.SizeFormatter.Format(fileInfo.Length)
                        });
                    }
                    catch
                    {
                        // Skip inaccessible files
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Cancelled
            }
            catch (UnauthorizedAccessException)
            {
                // Inaccessible root
            }
            catch (Exception)
            {
                // Other errors
            }

            return results
                .OrderByDescending(r => r.SizeBytes)
                .Take(topN)
                .ToList();
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, int>> GetFileAgeStatsAsync(
        string rootPath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Root path must not be null or empty.", nameof(rootPath));
        }

        var now = DateTime.Now;
        var buckets = new Dictionary<string, int>
        {
            ["0-30 days"] = 0,
            ["30-90 days"] = 0,
            ["90-365 days"] = 0,
            ["1-5 years"] = 0,
            ["5+ years"] = 0,
            ["Unknown"] = 0
        };

        return await Task.Run(() =>
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var writeTime = File.GetLastWriteTime(file);
                        var ageDays = (now - writeTime).TotalDays;

                        if (ageDays <= 30)
                        {
                            buckets["0-30 days"]++;
                        }
                        else if (ageDays <= 90)
                        {
                            buckets["30-90 days"]++;
                        }
                        else if (ageDays <= 365)
                        {
                            buckets["90-365 days"]++;
                        }
                        else if (ageDays <= 365 * 5)
                        {
                            buckets["1-5 years"]++;
                        }
                        else
                        {
                            buckets["5+ years"]++;
                        }
                    }
                    catch
                    {
                        buckets["Unknown"]++;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Cancelled
            }
            catch (Exception)
            {
                // Other errors
            }

            return buckets;
        }, cancellationToken);
    }

    private DateTime GetLastWriteTimeSafe(string path)
    {
        try
        {
            return File.GetLastWriteTime(path);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}
