using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Win11DeTrasher.Utils;

/// <summary>
/// Static helpers for UAC elevation. The application runs as asInvoker by default;
/// when the user opts in to scan protected paths, this helper re-launches the
/// application with elevated rights via ShellExecute's "runas" verb.
/// </summary>
public static class ElevationHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SHELLEXECUTEINFO
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpVerb;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpParameters;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpDirectory;
        public int nShow;
        public IntPtr hInstance;
        public IntPtr lpReserved;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpIdList;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpClass;
        public IntPtr hkeyClass;
        public uint dwFlags;
        public IntPtr hInstApp;
        public IntPtr lpcml;
        public IntPtr pvReserved;
        public IntPtr pvReserved2;
        public IntPtr hInstAppEx;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TOKEN_ELEVATION
    {
        public int TokenIsElevated;
    }

    private const uint TOKEN_QUERY = 0x0008;
    private const int TokenIsElevated = 12;
    private const uint SEE_MASK_NOCLOSEPROCESS = 0x40;
    private const int SW_SHOWNORMAL = 1;
    private const int ERROR_CANCELLED = 1223;

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool GetTokenInformation(
        IntPtr tokenHandle,
        int tokenInformationClass,
        IntPtr tokenInformation,
        int tokenInformationLength,
        out int returnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    /// <summary>
    /// Gets a value indicating whether the current process is running with elevated rights.
    /// </summary>
    public static bool IsElevated()
    {
        IntPtr tokenHandle = IntPtr.Zero;
        try
        {
            if (!OpenProcessToken(System.Diagnostics.Process.GetCurrentProcess().Handle, TOKEN_QUERY, out tokenHandle))
            {
                return false;
            }

            int returnLength = 4;
            var elevationPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(4);
            try
            {
                if (!GetTokenInformation(
                    tokenHandle,
                    TokenIsElevated,
                    elevationPtr,
                    4,
                    out returnLength))
                {
                    return false;
                }

                var elevation = System.Runtime.InteropServices.Marshal.ReadInt32(elevationPtr);
                return elevation != 0;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(elevationPtr);
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            if (tokenHandle != IntPtr.Zero)
            {
                CloseHandle(tokenHandle);
            }
        }
    }

    /// <summary>
    /// Launches a new instance of the current application with elevated rights.
    /// The current instance should exit after calling this method.
    /// </summary>
    /// <param name="arguments">Optional command-line arguments to pass to the elevated instance.</param>
    /// <returns>True if the elevated process was launched, false if the user cancelled the UAC prompt.</returns>
    public static bool LaunchElevated(string arguments = "")
    {
        var exePath = Environment.ProcessPath
            ?? throw new InvalidOperationException("Process path is not available.");

        var execInfo = new SHELLEXECUTEINFO
        {
            cbSize = Marshal.SizeOf<SHELLEXECUTEINFO>(),
            fMask = SEE_MASK_NOCLOSEPROCESS,
            lpVerb = "runas",
            lpFile = exePath,
            lpParameters = arguments,
            lpDirectory = Path.GetDirectoryName(exePath),
            nShow = SW_SHOWNORMAL
        };

        int result = ShellExecuteEx(ref execInfo);

        // HRESULT 0x800704C7 (ERROR_CANCELLED) means the user pressed Cancel
        if (result == unchecked((int)0x800704C7))
        {
            return false;
        }

        return result >= 0;
    }

    /// <summary>
    /// Checks whether the given path requires elevation to be accessed.
    /// </summary>
    public static bool RequiresElevation(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        var normalized = path.TrimEnd('\\').ToLowerInvariant();

        string[] protectedPrefixes =
        [
            "c:\\windows",
            "c:\\program files",
            "c:\\program files (x86)",
            "c:\\perflogs",
            "c:\\inetpub"
        ];

        foreach (var prefix in protectedPrefixes)
        {
            if (normalized == prefix || normalized.StartsWith(prefix + "\\"))
            {
                return true;
            }
        }

        return false;
    }
}
