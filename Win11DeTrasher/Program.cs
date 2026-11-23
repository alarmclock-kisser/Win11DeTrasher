using System.Text.Json;
using System.Text.Json.Nodes;
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
        warning = EnsureAppSettingsFile();
        var options = CreateDefaultLoggerOptions();

        var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        try
        {
            string settingsJson;
            if (File.Exists(settingsPath))
            {
                settingsJson = File.ReadAllText(settingsPath);
            }
            else
            {
                using var defaults = typeof(Program).Assembly.GetManifestResourceStream("Win11DeTrasher.appsettings.json");
                if (defaults is null)
                {
                    warning = JoinWarnings(warning, "No appsettings.json template was found; using built-in logger defaults.");
                    return options;
                }

                using var reader = new StreamReader(defaults);
                settingsJson = reader.ReadToEnd();
            }

            using var document = JsonDocument.Parse(settingsJson);
            if (!document.RootElement.TryGetProperty("LoggerSettings", out var loggerSection)
                || loggerSection.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException("The LoggerSettings object is missing from appsettings.json.");
            }

            options = JsonSerializer.Deserialize<RollingFileMemoryLoggerOptions>(
                loggerSection.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? options;

            var hasCurrentConfigVersion = document.RootElement.TryGetProperty("ConfigurationVersion", out var versionElement)
                && versionElement.TryGetInt32(out var configVersion)
                && configVersion >= 2;
            if (!hasCurrentConfigVersion)
            {
                // Older shipped appsettings used the relative "Logs" path.
                // Migrate that default to LocalAppData, but preserve explicit
                // custom paths and retention values.
                var migrateOldDefaults = string.Equals(options.LogDirectory, "Logs", StringComparison.OrdinalIgnoreCase);
                if (migrateOldDefaults)
                {
                    options.LogDirectory = DefaultLogDirectorySetting;
                    options.MaxLogFiles = 16;
                    warning = JoinWarnings(warning, "Migrated the previous default logger path to LocalAppData and set retention to 16 files.");
                }

                try
                {
                    PersistMigratedAppSettings(settingsPath, settingsJson, migrateOldDefaults);
                }
                catch (Exception ex)
                {
                    warning = JoinWarnings(warning, $"Could not save the migrated appsettings.json ({ex.Message}).");
                }
            }

            options.LogDirectory = string.IsNullOrWhiteSpace(options.LogDirectory)
                ? DefaultLogDirectory
                : Environment.ExpandEnvironmentVariables(options.LogDirectory);
            if (!Path.IsPathRooted(options.LogDirectory))
            {
                options.LogDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, options.LogDirectory));
            }

            return options;
        }
        catch (Exception ex)
        {
            warning = JoinWarnings(warning, $"Could not read appsettings.json ({ex.Message}); using built-in logger defaults.");
            options = CreateDefaultLoggerOptions();
            return options;
        }
    }

    private static RollingFileMemoryLoggerOptions CreateDefaultLoggerOptions() => new()
    {
        LogDirectory = DefaultLogDirectory,
        CreateLogFile = true,
        MaxLogFiles = 16,
        MaxLogEntries = 20_000,
        LogTimestampFormat = "HH:mm:ss.fff"
    };

    private static string DefaultLogDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Win11DeTrasher",
        "Logs");

    private static string DefaultLogDirectorySetting => "%LOCALAPPDATA%\\Win11DeTrasher\\Logs";

    private static string? EnsureAppSettingsFile()
    {
        var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(settingsPath))
        {
            return null;
        }

        try
        {
            using var defaults = typeof(Program).Assembly.GetManifestResourceStream("Win11DeTrasher.appsettings.json")
                ?? throw new InvalidOperationException("The embedded appsettings.json template is missing.");
            using var destination = new FileStream(settingsPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            defaults.CopyTo(destination);
            return null;
        }
        catch (IOException) when (File.Exists(settingsPath))
        {
            // Another process created the file after the initial check.
            return null;
        }
        catch (Exception ex)
        {
            return $"Could not create appsettings.json beside the executable ({ex.Message}); using embedded/default settings.";
        }
    }

    private static void PersistMigratedAppSettings(string settingsPath, string settingsJson, bool migrateOldDefaults)
    {
        if (!File.Exists(settingsPath))
        {
            return;
        }

        if (JsonNode.Parse(settingsJson) is not JsonObject root
            || root["LoggerSettings"] is not JsonObject loggerSettings)
        {
            throw new JsonException("Could not migrate the LoggerSettings object.");
        }

        root["ConfigurationVersion"] = 2;
        if (migrateOldDefaults)
        {
            loggerSettings["LogDirectory"] = DefaultLogDirectorySetting;
            loggerSettings["MaxLogFiles"] = 16;
        }
        File.WriteAllText(settingsPath, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    private static string? JoinWarnings(string? first, string second)
    {
        return string.IsNullOrWhiteSpace(first) ? second : $"{first} {second}";
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
