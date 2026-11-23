namespace Win11DeTrasher.Models;

/// <summary>
/// A named preset of scan filter options, persisted as JSON in the user's
/// AppData directory. Allows saving and loading frequently used filter configurations.
/// </summary>
/// <param name="Name">The preset name (e.g. "ONNX DLLs", "Large files only", "Everything").</param>
/// <param name="Options">The scan filter options stored in this preset.</param>
public sealed record ScanFilterPreset(
    string Name,
    ScanFilterOptions Options);
