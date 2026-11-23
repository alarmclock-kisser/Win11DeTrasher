namespace Win11DeTrasher.Models;

/// <summary>
/// Represents a group of files that are byte-identical (same SHA256 hash).
/// The potential savings is the size of one copy (the rest are duplicates).
/// </summary>
/// <param name="GroupId">The unique group id assigned by the duplication detector.</param>
/// <param name="Hash">The SHA256 hash shared by all files in this group.</param>
/// <param name="FileSizeBytes">The size in bytes of each file in this group.</param>
/// <param name="FileCount">The number of files in this group.</param>
/// <param name="PotentialSavingsBytes">The potential space savings in bytes (FileSizeBytes * (FileCount - 1)).</param>
/// <param name="Files">The list of file paths in this group.</param>
public sealed record DuplicateGroup(
    int GroupId,
    string Hash,
    long FileSizeBytes,
    int FileCount,
    long PotentialSavingsBytes,
    List<string> Files);
