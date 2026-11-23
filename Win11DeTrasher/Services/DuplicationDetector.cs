using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.IO;
using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Default implementation of <see cref="IDuplicationDetector"/>.
/// Uses a two-phase approach: first groups files by size (fast, in-memory),
/// then hashes only files in groups with 2+ members (SHA256).
/// Hashing is parallelized across multiple threads for performance.
/// </summary>
public class DuplicationDetector : IDuplicationDetector
{
    private const int HashParallelism = 4;
    private const int BufferSize = 81920; // 80 KB read buffer for hashing

    /// <inheritdoc/>
    public async Task<IList<DuplicateGroup>> DetectDuplicatesAsync(
        IEnumerable<ScanResult> scanResults,
        long minSizeBytes,
        CancellationToken cancellationToken)
    {
        if (scanResults == null)
        {
            throw new ArgumentNullException(nameof(scanResults));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Phase 1: Filter files and group by size
        var sizeGroups = this.GroupBySize(scanResults, minSizeBytes);

        // Keep only groups with 2+ files (potential duplicates)
        var candidateGroups = sizeGroups
            .Where(g => g.Value.Count >= 2)
            .Select(g => g.Value)
            .ToList();

        if (candidateGroups.Count == 0)
        {
            return [];
        }

        // Phase 2: Hash candidate files in parallel
        var hashResults = await this.HashCandidatesInParallelAsync(candidateGroups, cancellationToken);

        // Phase 3: Group by hash
        var hashGroups = hashResults
            .GroupBy(r => r.Hash)
            .Where(g => g.Count() >= 2)
            .ToList();

        // Build DuplicateGroup records
        var duplicateGroups = new List<DuplicateGroup>();
        int groupId = 1;

        foreach (var hashGroup in hashGroups)
        {
            var first = hashGroup.First();
            var potentialSavings = first.SizeBytes * (hashGroup.Count() - 1);

            duplicateGroups.Add(new DuplicateGroup(
                GroupId: groupId++,
                Hash: first.Hash,
                FileSizeBytes: first.SizeBytes,
                FileCount: hashGroup.Count(),
                PotentialSavingsBytes: potentialSavings,
                Files: hashGroup.Select(r => r.FullPath).ToList()));
        }

        // Sort by potential savings (largest first)
        return duplicateGroups
            .OrderByDescending(g => g.PotentialSavingsBytes)
            .ToList();
    }

    private record CandidateFile(string FullPath, long SizeBytes, string Hash);

    private Dictionary<long, List<ScanResult>> GroupBySize(
        IEnumerable<ScanResult> scanResults,
        long minSizeBytes)
    {
        var groups = new Dictionary<long, List<ScanResult>>();

        foreach (var result in scanResults)
        {
            // Skip directories
            if (result.IsDirectory)
            {
                continue;
            }

            // Skip files below the minimum size threshold
            if (result.SizeBytes < minSizeBytes)
            {
                continue;
            }

            // Skip files with 0 bytes (empty files are trivially "duplicates" but useless)
            if (result.SizeBytes == 0)
            {
                continue;
            }

            if (!groups.TryGetValue(result.SizeBytes, out var list))
            {
                list = [];
                groups[result.SizeBytes] = list;
            }

            list.Add(result);
        }

        return groups;
    }

    private async Task<List<CandidateFile>> HashCandidatesInParallelAsync(
        List<List<ScanResult>> candidateGroups,
        CancellationToken cancellationToken)
    {
        var allCandidates = candidateGroups.SelectMany(g => g).ToList();
        var results = new ConcurrentBag<CandidateFile>();
        var semaphore = new SemaphoreSlim(HashParallelism);

        var tasks = allCandidates.Select(async file =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hash = this.ComputeSha256(file.FullPath, cancellationToken);
                if (hash != null)
                {
                    results.Add(new CandidateFile(file.FullPath, file.SizeBytes, hash));
                }
            }
            catch (OperationCanceledException)
            {
                // Cancelled
            }
            catch (Exception)
            {
                // File could not be hashed (in use, permission denied, etc.)
                // Skip silently
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        return results.ToList();
    }

    /// <summary>
    /// Computes the SHA256 hash of a file. Returns null when the file
    /// could not be read (in use, permission denied, etc.).
    /// </summary>
    private string? ComputeSha256(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            using var sha256 = SHA256.Create();

            // Read the file in chunks to avoid loading large files into memory
            var buffer = new byte[BufferSize];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, BufferSize)) > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
            }
            sha256.TransformFinalBlock(buffer, 0, 0);

            return Convert.ToHexString(sha256.Hash!).ToLowerInvariant();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
