using System.Text.Json;
using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Default implementation of <see cref="IConfigService"/>.
/// Persists filter presets as JSON in %AppData%/Win11DeTrasher/filters.json.
/// </summary>
public class ConfigService : IConfigService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <inheritdoc/>
    public string PresetsFilePath
    {
        get
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDir = Path.Combine(appDataPath, "Win11DeTrasher");
            return Path.Combine(appDir, "filters.json");
        }
    }

    /// <inheritdoc/>
    public IList<ScanFilterPreset> LoadPresets()
    {
        try
        {
            var path = this.PresetsFilePath;
            if (!File.Exists(path))
            {
                return [];
            }

            var json = File.ReadAllText(path);
            var presets = JsonSerializer.Deserialize<List<ScanFilterPreset>>(json);
            return presets ?? [];
        }
        catch
        {
            return [];
        }
    }

    /// <inheritdoc/>
    public bool SavePresets(IList<ScanFilterPreset> presets)
    {
        try
        {
            var path = this.PresetsFilePath;
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(presets, JsonOptions);
            File.WriteAllText(path, json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
