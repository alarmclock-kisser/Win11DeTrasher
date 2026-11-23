using System.IO;
using Win11DeTrasher.Models;
using Win11DeTrasher.Utils;

namespace Win11DeTrasher.Services;

/// <summary>
/// Default implementation of <see cref="ICleanupService"/>.
/// Uses the Windows Shell API (SHFileOperation) for Recycle Bin operations
/// and <see cref="Directory.Delete"/>/<see cref="File.Delete"/> for hard deletes.
/// </summary>
public class CleanupService : ICleanupService
{
    /// <inheritdoc/>
    public Task<BatchCleanupResult> SendToRecycleBinAsync(
        IList<string> paths,
        CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var results = new List<CleanupResult>();

            foreach (var path in paths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = this.TrySendToRecycleBin(path);
                results.Add(result);
            }

            return this.BuildBatchResult(results);
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<BatchCleanupResult> HardDeleteAsync(
        IList<string> paths,
        CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var results = new List<CleanupResult>();

            foreach (var path in paths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = this.TryHardDelete(path);
                results.Add(result);
            }

            return this.BuildBatchResult(results);
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public long GetTotalSize(IList<string> paths)
    {
        long total = 0;

        foreach (var path in paths)
        {
            try
            {
                if (File.Exists(path))
                {
                    total += new FileInfo(path).Length;
                }
                else if (Directory.Exists(path))
                {
                    foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        total += new FileInfo(file).Length;
                    }
                }
            }
            catch
            {
                // Inaccessible path
            }
        }

        return total;
    }

    private CleanupResult TrySendToRecycleBin(string path)
    {
        try
        {
            // Use SHFileOperation with FO_DELETE and FOF_ALLOWUNDO (Recycle Bin)
            var op = new SHFILEOPSTRUCT
            {
                hwnd = IntPtr.Zero,
                wFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_SILENT,
                pFrom = path + "\0",
                pTo = null,
                fOperation = FO_DELETE
            };

            int result = SHFileOperation(ref op);

            if (result == 0)
            {
                var freedBytes = this.GetPathSize(path);
                return new CleanupResult(path, true, null, freedBytes);
            }
            else
            {
                return new CleanupResult(path, false, $"Shell operation failed (code: {result})", 0);
            }
        }
        catch (Exception ex)
        {
            return new CleanupResult(path, false, ex.Message, 0);
        }
    }

    private CleanupResult TryHardDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                var size = new FileInfo(path).Length;
                File.Delete(path);
                return new CleanupResult(path, true, null, size);
            }
            else if (Directory.Exists(path))
            {
                var size = this.GetDirectorySize(path);
                Directory.Delete(path, recursive: true);
                return new CleanupResult(path, true, null, size);
            }
            else
            {
                return new CleanupResult(path, false, "Path does not exist", 0);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            return new CleanupResult(path, false, $"Access denied: {ex.Message}", 0);
        }
        catch (Exception ex)
        {
            return new CleanupResult(path, false, ex.Message, 0);
        }
    }

    private long GetPathSize(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                return new FileInfo(path).Length;
            }

            return this.GetDirectorySize(path);
        }
        catch
        {
            return 0;
        }
    }

    private long GetDirectorySize(string path)
    {
        long total = 0;

        try
        {
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                total += new FileInfo(file).Length;
            }
        }
        catch
        {
            // Inaccessible directory
        }

        return total;
    }

    private BatchCleanupResult BuildBatchResult(List<CleanupResult> results)
    {
        var succeeded = results.Count(r => r.Success);
        var failed = results.Count(r => !r.Success);
        var totalFreed = results.Sum(r => r.FreedBytes);

        return new BatchCleanupResult(
            results.Count,
            succeeded,
            failed,
            totalFreed,
            results);
    }

    // --- P/Invoke for SHFileOperation ---

    private const int FO_DELETE = 3;
    private const int FOF_SILENT = 0x4;
    private const int FOF_NOCONFIRMATION = 0x10;
    private const int FOF_ALLOWUNDO = 0x40;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private struct SHFILEOPSTRUCT
    {
        public IntPtr hwnd;
        public uint pReserved;
        public uint wFlags;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string? pFrom;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string? pTo;
        public int fOperation;
    }

    [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern int SHFileOperation(ref SHFILEOPSTRUCT pshfo);
}
