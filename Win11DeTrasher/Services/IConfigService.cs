using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Abstraction for persisting scan filter presets as JSON files
/// in the user's AppData directory.
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Gets the full path to the filter presets file.
    /// The file is located in %AppData%/Win11DeTrasher/filters.json.
    /// </summary>
    string PresetsFilePath { get; }

    /// <summary>
    /// Loads all saved filter presets from the presets file.
    /// Returns an empty list when the file does not exist or is invalid.
    /// </summary>
    /// <returns>A list of saved <see cref="ScanFilterPreset"/> instances.</returns>
    IList<ScanFilterPreset> LoadPresets();

    /// <summary>
    /// Saves the given list of filter presets to the presets file,
    /// replacing any existing content.
    /// </summary>
    /// <param name="presets">The list of presets to save.</param>
    /// <returns>True if the save succeeded, false otherwise.</returns>
    bool SavePresets(IList<ScanFilterPreset> presets);
}
