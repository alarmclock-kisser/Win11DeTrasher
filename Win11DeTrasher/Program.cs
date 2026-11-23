using Win11DeTrasher.Shared;
using System.Text.Json;

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

        var loggerOptions = LoadLoggerOptions(out var configurationWarning);
        var logger = CreateLoggerWithFallback(loggerOptions, out var logDirectoryWarning);
        logger.StartBackgroundWriter(CancellationToken.None);
        if (configurationWarning is not null)
        {
            logger.LogWarning(configurationWarning);
        }
        if (logDirectoryWarning is not null)
        {
            logger.LogWarning(logDirectoryWarning);
        }
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

    private static RollingFileMemoryLoggerOptions LoadLoggerOptions(out string? warning)
    {
        warning = null;
        var options = new RollingFileMemoryLoggerOptions
        {
            LogDirectory = "Logs",
            CreateLogFile = true,
            MaxLogFiles = 20,
            MaxLogEntries = 20_000,
            LogTimestampFormat = "HH:mm:ss.fff"
        };

        var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        try
        {
            using var settingsStream = File.Exists(settingsPath)
                ? File.OpenRead(settingsPath)
                : typeof(Program).Assembly.GetManifestResourceStream("Win11DeTrasher.appsettings.json");
            if (settingsStream is null)
            {
                warning = "No appsettings.json was found; using built-in logger defaults.";
                return options;
            }

            using var document = JsonDocument.Parse(settingsStream);
            if (!document.RootElement.TryGetProperty("LoggerSettings", out var loggerSection)
                || loggerSection.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException("The LoggerSettings object is missing from appsettings.json.");
            }

            options = JsonSerializer.Deserialize<RollingFileMemoryLoggerOptions>(
                loggerSection.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? options;

            options.LogDirectory = string.IsNullOrWhiteSpace(options.LogDirectory)
                ? "Logs"
                : Environment.ExpandEnvironmentVariables(options.LogDirectory);
            if (!Path.IsPathRooted(options.LogDirectory))
            {
                options.LogDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, options.LogDirectory));
            }

            return options;
        }
        catch (Exception ex)
        {
            warning = $"Could not read appsettings.json ({ex.Message}); using built-in logger defaults.";
            options.LogDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Logs"));
            return options;
        }
    }

    private static RollingFileMemoryLogger CreateLoggerWithFallback(
        RollingFileMemoryLoggerOptions options,
        out string? warning)
    {
        var configuredDirectory = options.LogDirectory;
        var logger = new RollingFileMemoryLogger(options);
        warning = null;

        if (options.CreateLogFile && logger.LogFilePath is null)
        {
            options.LogDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Win11DeTrasher",
                "Logs");
            logger = new RollingFileMemoryLogger(options);
            warning = $"Could not create the configured log in '{configuredDirectory}'. " +
                $"Using '{options.LogDirectory}' instead.";
        }

        return logger;
    }
}
