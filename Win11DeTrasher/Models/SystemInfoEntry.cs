namespace Win11DeTrasher.Models;

/// <summary>
/// Represents a system info category (e.g. Temp folder, Recycle Bin, WinSxS)
/// with its size and a description, for display in the system info panel.
/// </summary>
/// <param name="Category">The category name (e.g. "Windows Temp", "User Temp", "Recycle Bin").</param>
/// <param name="Path">The file system path of this category, or an empty string when not applicable.</param>
/// <param name="SizeBytes">The total size in bytes.</param>
/// <param name="Description">A short human-readable description of this category.</param>
public sealed record SystemInfoEntry(
    string Category,
    string Path,
    long SizeBytes,
    string Description);
