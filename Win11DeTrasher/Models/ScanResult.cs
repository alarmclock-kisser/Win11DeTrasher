namespace Win11DeTrasher.Models;

/// <summary>
/// Represents a single file or directory discovered during a scan.
/// All size values are in bytes.
/// </summary>
public class ScanResult
{
    /// <summary>Gets or sets the full path of the file or directory.</summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>Gets or sets the name of the file or directory (without extension for files).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the file extension (e.g. ".dll"), or an empty string for directories.</summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>Gets or sets the size in bytes. For directories, this is the sum of all contained file sizes.</summary>
    public long SizeBytes { get; set; }

    /// <summary>Gets or sets the depth of this entry relative to the scan root. The root itself has depth 0.</summary>
    public int Depth { get; set; }

    /// <summary>Gets a value indicating whether this entry is a directory (true) or a file (false).</summary>
    public bool IsDirectory { get; set; }

    /// <summary>Gets or sets the last write time of the file or directory.</summary>
    public DateTime LastWriteTime { get; set; }

    /// <summary>Gets or sets the number of files contained in this directory, or 0 for files.</summary>
    public int FileCount { get; set; }

    /// <summary>Gets or sets the number of subdirectories contained in this directory, or 0 for files.</summary>
    public int DirectoryCount { get; set; }

    /// <summary>Gets or sets the SHA256 hash of the file content, or null when not computed (e.g. for directories or when duplicate detection is disabled).</summary>
    public string? Hash { get; set; }

    /// <summary>Gets or sets the group id of this file within a duplicate group, or 0 when this file is not part of any duplicate group.</summary>
    public int DuplicateGroupId { get; set; }

    /// <summary>Gets a value indicating whether this file is part of a duplicate group (has at least one other file with the same hash).</summary>
    public bool IsDuplicate => this.DuplicateGroupId > 0;

    /// <summary>Gets or sets a human-readable size string (e.g. "1.23 GB"), cached for display purposes.</summary>
    public string SizeDisplay { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of subdirectory paths contained in this directory.
    /// Only populated for directory entries; used by the scanner to enqueue
    /// subdirectories for further processing. Not serialized to reports.
    /// </summary>
    public List<string> SubdirectoryPaths { get; set; } = [];
}
