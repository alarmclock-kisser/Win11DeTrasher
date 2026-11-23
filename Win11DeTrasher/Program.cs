namespace Win11DeTrasher;

/// <summary>
/// Main entry point for the Win11DeTrasher application.
/// Initializes Windows Forms with high-DPI settings and launches the main form.
/// </summary>
static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var configService = new Services.ConfigService();
        var driveService = new Services.DriveDiscoveryService();
        var progressTracker = new Services.ProgressTracker();
        var scanner = new Services.FileScanner(progressTracker);
        var duplicationDetector = new Services.DuplicationDetector();
        var systemInfoService = new Services.SystemInfoService();
        var reportExporter = new Services.ReportExporter();
        var cleanupService = new Services.CleanupService();

        Application.Run(new MainForm(
            configService,
            driveService,
            progressTracker,
            scanner,
            duplicationDetector,
            systemInfoService,
            reportExporter,
            cleanupService));
    }
}
