using Win11DeTrasher.Shared;

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
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        var loggerOptions = new RollingFileMemoryLoggerOptions
        {
            LogDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Win11DeTrasher",
                "Logs"),
            CreateLogFile = true,
            MaxLogFiles = 20,
            MaxLogEntries = 20_000,
            LogTimestampFormat = "HH:mm:ss.fff"
        };
        var logger = new RollingFileMemoryLogger(loggerOptions);
        logger.StartBackgroundWriter(CancellationToken.None);
        logger.LogInfo("Win11DeTrasher process initialized.");
        Application.ThreadException += (_, args) => logger.Log(args.Exception, preText: "Unhandled UI-thread exception");
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                logger.Log(exception, preText: "Unhandled application-domain exception");
            }
            else
            {
                logger.LogError($"Unhandled application-domain failure: {args.ExceptionObject}");
            }
        };
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            logger.Log(args.Exception, preText: "Unobserved background-task exception");
            args.SetObserved();
        };

        var configService = new Services.ConfigService();
        var driveService = new Services.DriveDiscoveryService();
        var progressTracker = new Services.ProgressTracker();
        var scanner = new Services.FileScanner(progressTracker, logger);
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
            cleanupService,
            logger));
    }
}
