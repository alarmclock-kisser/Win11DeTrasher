# Win11DeTrasher — Technical Documentation

> **Version:** 1.0
> **Last updated:** 2026-11-23
> **Target audience:** Developers

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Project Structure](#2-project-structure)
3. [Core Components](#3-core-components)
4. [Scan Engine Design](#4-scan-engine-design)
5. [Progress Tracking](#5-progress-tracking)
6. [Filter Pipeline](#6-filter-pipeline)
7. [Duplicate Detection Algorithm](#7-duplicate-detection-algorithm)
8. [UAC Elevation](#8-uac-elevation)
9. [Cleanup Operations](#9-cleanup-operations)
10. [Report Export](#10-report-export)
11. [Configuration and Presets](#11-configuration-and-presets)
12. [UI Architecture](#12-ui-architecture)
13. [Threading Model](#13-threading-model)
14. [Error Handling Strategy](#14-error-handling-strategy)
15. [Testing Strategy](#15-testing-strategy)
16. [Build and Run](#16-build-and-run)
17. [Design Decisions and Trade-offs](#17-design-decisions-and-trade-offs)
18. [Performance Characteristics](#18-performance-characteristics)
19. [Appendix — API Reference](#19-appendix--api-reference)

---

## 1. Architecture Overview

Win11DeTrasher follows a **layered architecture** with clear separation
of concerns:

```
┌─────────────────────────────────────────────────────────┐
│  UI Layer (WinForms)                                    │
│  MainForm, ScanFilterForm, Controls/*, listBox_log       │
│  ┌───────────────────────────────────────────────────┐  │
│  │  Event handlers, data binding, user interaction   │  │
│  └───────────────────────────────────────────────────┘  │
├─────────────────────────────────────────────────────────┤
│  Service Layer (abstractions + implementations)         │
│  IFileScanner → FileScanner                            │
│  IProgressTracker → ProgressTracker                    │
│  IDuplicationDetector → DuplicationDetector            │
│  ISystemInfoService → SystemInfoService                │
│  IReportExporter → ReportExporter                      │
│  ICleanupService → CleanupService                      │
│  IConfigService → ConfigService                        │
│  IDriveDiscoveryService → DriveDiscoveryService        │
├─────────────────────────────────────────────────────────┤
│  Model Layer (data transfer objects)                    │
│  ScanResult, ScanFilterOptions, ScanProgress,           │
│  SystemInfoEntry, DuplicateGroup, CleanupResult,        │
│  ScanFilterPreset                                      │
├─────────────────────────────────────────────────────────┤
│  Utility Layer (static helpers)                         │
│  ElevationHelper, SizeFormatter, RoFM utilities          │
└─────────────────────────────────────────────────────────┘
```

**Key principles:**

- **Dependency Inversion:** The UI depends on service interfaces,
  not concrete classes. Services are injected via the `MainForm`
  constructor.
- **Single Responsibility:** Each class has one reason to change.
- **Open/Closed:** New service implementations can be added without
  modifying existing code (e.g. a new `IReportExporter` implementation
  for a different format).
- **Testability:** All services are behind interfaces, enabling
  mocking in unit tests.

## 2. Project Structure

```
Win11DeTrasher/
├── Win11DeTrasher.slnx          # Solution file
├── .gitignore
├── requirements.md              # Living requirements document
├── user_manual.md               # User-facing guide
├── technical_documentation.md   # This file
├── Win11DeTrasher/              # Main application project
│   ├── Win11DeTrasher.csproj    # net10.0-windows, win-x64, WinExe
│   ├── app.manifest             # asInvoker, PerMonitorV2 DPI
│   ├── appsettings.json         # External RoFML logger configuration
│   ├── Program.cs               # Entry point, DI wiring
│   ├── MainForm.cs              # Main form event handlers
│   ├── MainForm.Designer.cs     # UI layout (Designer)
│   ├── Controls/
│   │   ├── SortableDataGridView.cs  # Custom sort cycling
│   │   └── WorkerControl.cs         # Worker count UI
│   ├── UI/
│   │   └── ScanFilterForm.cs        # Filter config dialog
│   ├── Services/
│   │   ├── IDriveDiscoveryService.cs / DriveDiscoveryService.cs
│   │   ├── IFileScanner.cs / FileScanner.cs
│   │   ├── IProgressTracker.cs / ProgressTracker.cs
│   │   ├── IDuplicationDetector.cs / DuplicationDetector.cs
│   │   ├── ISystemInfoService.cs / SystemInfoService.cs
│   │   ├── IReportExporter.cs / ReportExporter.cs
│   │   ├── ICleanupService.cs / CleanupService.cs
│   │   └── IConfigService.cs / ConfigService.cs
│   ├── Models/
│   │   ├── ScanResult.cs
│   │   ├── ScanFilterOptions.cs
│   │   ├── ScanProgress.cs
│   │   ├── SystemInfoEntry.cs
│   │   ├── DuplicateGroup.cs
│   │   ├── CleanupResult.cs
│   │   └── ScanFilterPreset.cs
│   ├── Shared/
│   │   └── RoFMLogger.cs          # IRollingFileMemoryLogger + logger + included helpers
│   └── Utils/
│       ├── ElevationHelper.cs
│       └── SizeFormatter.cs
└── Win11DeTrasher.Tests/        # MSTest project
    ├── Win11DeTrasher.Tests.csproj
    ├── GlobalUsings.cs
    ├── TestBase.cs
    ├── SizeFormatterTests.cs
    ├── ProgressTrackerTests.cs
    ├── FileScannerTests.cs
    ├── DuplicationDetectorTests.cs
    ├── ReportExporterTests.cs
    ├── ConfigServiceTests.cs
    ├── DriveDiscoveryServiceTests.cs
    └── CleanupServiceTests.cs
```

## 3. Core Components

### 3.1 ScanResult

The central data type. Represents a single file or directory discovered
during a scan.

```csharp
public class ScanResult
{
    public string FullPath { get; set; }
    public string Name { get; set; }
    public string Extension { get; set; }
    public long SizeBytes { get; set; }
    public int Depth { get; set; }
    public bool IsDirectory { get; set; }
    public DateTime LastWriteTime { get; set; }
    public int FileCount { get; set; }       // for directories
    public int DirectoryCount { get; set; }  // for directories
    public string? Hash { get; set; }        // SHA256, if computed
    public int DuplicateGroupId { get; set; } // 0 = not a duplicate
    public bool IsDuplicate => DuplicateGroupId > 0;
    public string SizeDisplay { get; set; }  // cached display string
    public List<string> SubdirectoryPaths { get; set; } // for scanner
}
```

**Notes:**
- `SubdirectoryPaths` is a transient property used by the scanner to
  enqueue subdirectories. It is not serialized to reports.
- `SizeDisplay` is a cached human-readable string to avoid repeated
  formatting in the UI.
- `Hash` and `DuplicateGroupId` are only populated when duplicate
  detection is enabled.

### 3.2 ScanFilterOptions

Configuration for a scan. Serializable to JSON for preset storage.

```csharp
public class ScanFilterOptions
{
    public int MaxDepth { get; set; } = -1;
    public List<string> IncludeExtensions { get; set; } = [];
    public List<string> ExcludeExtensions { get; set; } = [];
    public List<string> IncludePathSegments { get; set; } = [];
    public List<string> ExcludePathSegments { get; set; } = [];
    public long MinFileSizeBytes { get; set; } = 0;
    public long MaxFileSizeBytes { get; set; } = long.MaxValue;
    public bool SkipHidden { get; set; } = true;
    public bool SkipSystem { get; set; } = false;
    public bool DetectDuplicates { get; set; } = false;
    public long DuplicateMinSizeBytes { get; set; } = 1048576;
}
```

All list-based filters are **case-insensitive** when matched.
Empty lists mean "no filter" (all values pass).

### 3.3 ScanProgress

A `readonly record struct` — a lightweight, immutable snapshot of
scan progress. Used for thread-safe publication to the UI.

```csharp
public readonly record struct ScanProgress(
    long TotalBytes,
    long ProcessedBytes,
    int TotalFiles,
    int ProcessedFiles,
    double ElapsedSeconds,
    double EstimatedRemainingSeconds,
    double ProgressPercent,
    bool IsCompleted,
    bool IsCancelled,
    int ErrorCount);
```

**Why a record struct?**
- Value type: no heap allocation per update.
- `readonly`: immutable — safe to pass across threads without
  synchronization.
- `record`: value-based equality for testing.

## 4. Scan Engine Design

### 4.1 Work Queue Pattern

The scanner uses a **producer-consumer work queue** pattern:

```
┌──────────────┐
│  Work Queue  │  BlockingCollection<string>
│  (paths)     │  Bounded: WorkerCount * 4
└──────┬───────┘
       │ dequeue
       ▼
┌──────────────────────────────────────────────────┐
│  Worker 1    │  Worker 2    │  Worker N         │
│  ProcessPath │  ProcessPath │  ProcessPath      │
│  Enqueue sub │  Enqueue sub │  Enqueue sub      │
└──────────────┘  └───────────┘  └────────────────┘
       │
       │ batch (every 500 items)
       ▼
┌──────────────┐
│ Shared State │  lock-protected results list
│ (results)    │  ProgressTracker
└──────────────┘
```

**Key design decisions:**

1. **`BlockingCollection<string>`** as the work queue:
   - Thread-safe enqueue/dequeue.
   - `CompleteAdding()` signals workers to drain and exit.
   - Bounded size (`WorkerCount * 4`) limits shared-queue growth.
     When full, a producing worker keeps overflow paths in a local
     stack instead of blocking while it is the only consumer.

2. **Batching:** Each worker accumulates results locally and flushes
   to the shared list every 500 items. This minimizes lock contention
   while keeping memory bounded.

3. **Subdirectory enqueueing:** When a worker processes a directory,
   it enqueues all subdirectory paths. Any worker can pick them up —
   this provides natural load balancing.

4. **Pending-directory completion counter:** Because subdirectories
   are discovered dynamically (a worker enqueues them while processing),
   `CompleteAdding()` cannot be called upfront — the queue would be
   completed before workers have a chance to enqueue subdirectories.
   Instead, a thread-safe counter (`Interlocked` increment/decrement)
    tracks how many directories are "pending" (enqueued or locally held,
    but not yet fully processed). The counter is incremented before a
    child becomes visible to another worker;
   each directory fully processed (all subdirs enqueued) decrements it.
   When the counter reaches zero, `CompleteAdding()` is called, and
   workers drain the remaining queue and exit. A safety-net
    `CompleteAdding()` in the `finally` block of `AwaitCompletionAsync`
    guarantees workers can always exit, even if an error prevents the
    counter from reaching zero.

5. **Dynamic worker finalization:** Worker tasks added during a scan are
   included in the final wait. Once directory traversal completes (or
   cancellation is requested), the scanner stops accepting worker-count
   changes before collecting the final task set.

6. **Selected-root attribute handling:** The explicitly selected root is
   traversed even when Windows marks it Hidden or System (common for volume
   roots). Hidden/System filters apply to discovered entries below that root.

### 4.2 On-the-Fly Worker Adjustment

The `WorkerCount` property can be changed during a running scan:

```csharp
public int WorkerCount
{
    get { lock (_sync) return _targetWorkerCount; }
    set
    {
        var clamped = Math.Max(1, Math.Min(value, Environment.ProcessorCount));
        lock (_sync)
        {
            _targetWorkerCount = clamped;
            if (_isScanning)
            {
                AdaptWorkerPool(clamped);
            }
        }
    }
}
```

**How it works:**
- **Increasing count:** New worker tasks are started immediately.
  They begin consuming from the shared queue.
- **Decreasing count:** No workers are forcibly terminated. Excess
  workers will finish their current item and exit when the queue
  is empty. This avoids data loss and is safe.

**Why not forcibly stop workers?**
- A worker mid-way through processing a file could leave the
  results list in an inconsistent state.
- The natural drain approach is simpler and safer.
- In practice, the queue drains quickly, so the "excess" workers
  exit within seconds.

### 4.3 Cancellation

A `CancellationTokenSource` is created for each scan, linked to
the external token. When canceled:
- Workers check the token on each dequeue (`GetConsumingEnumerable`
  throws `OperationCanceledException`).
- The `AwaitCompletionAsync` method catches the exception and
  reports cancellation to the progress tracker.
- The results collected up to that point are returned.

### 4.4 Thread Safety

| Shared State | Protection |
|---|---|
| `_results` (List<ScanResult>) | `lock(_sync)` on flush |
| `_workQueue` (BlockingCollection) | Inherently thread-safe |
| `_progressTracker` state | `lock` inside ProgressTracker |
| `_isScanning` flag | `lock(_sync)` |
| `_workerTasks` (List<Task>) | `lock(_sync)` |

## 5. Progress Tracking

### 5.1 Design

The `ProgressTracker` is a thread-safe class that:
- Maintains counters (total/processed bytes/files, errors).
- Computes progress percentage and ETA on each snapshot.
- Publishes `ScanProgress` via an event, throttled to 5 per second.

### 5.2 ETA Calculation

```
rate = processedBytes / elapsedSeconds
ETA  = (totalBytes - processedBytes) / rate
```

**Caveats:**
- ETA is unreliable in the first few seconds (rate is not yet stable).
- If `totalBytes` is still growing (files still being discovered),
  the ETA will fluctuate. This is expected — the progress bar
  reflects the ratio of processed to discovered, not of processed
  to the final total.
- ETA is shown as "calculating..." until `elapsed > 1s` and
  `0 < processedBytes < totalBytes`.

### 5.3 Throttling

```csharp
private const double MinEventIntervalSeconds = 0.2;
// 5 events per second maximum
```

This prevents the UI thread from being flooded with updates during
high-throughput scans. The progress bar updates smoothly because
WinForms coalesces paint calls.

## 6. Filter Pipeline

The filter pipeline is applied per file/directory, in this order:

```
1. Hidden/System attribute check
2. Depth limit (directories only)
3. Min/Max file size (files only)
4. Extension include (if list non-empty)
5. Extension exclude (if list non-empty)
6. Path segment include (if list non-empty)
7. Path segment exclude (if list non-empty)
```

**Short-circuit:** If any filter rejects the file, processing stops
and `null` is returned (the file is not added to results).

**Case sensitivity:** All string comparisons (extensions, path
segments) use `StringComparison.OrdinalIgnoreCase`.

**Relative path computation:** The relative path is computed as
`path[rootPath.Length..].TrimStart('\\').ToLowerInvariant()`.
This is used for path segment matching.

## 7. Duplicate Detection Algorithm

### 7.1 Two-Phase Approach

```
Phase 1: Group by size (in-memory, O(n))
  ┌─────────────────────────────────┐
  │ size=11: [dup1, dup2, dup3]   │  ← candidate (3 files)
  │ size=19: [unique1, unique2]   │  ← candidate (2 files)
  │ size=25: [only1]             │  ← skip (1 file)
  └─────────────────────────────────┘

Phase 2: Hash candidates in parallel (O(k), k = candidate count)
  ┌─────────────────────────────────┐
  │ dup1: a3f5... → group 1       │
  │ dup2: a3f5... → group 1       │  ← confirmed duplicate
  │ dup3: a3f5... → group 1       │
  │ unique1: b7c2... → group 2    │
  │ unique2: d9e1... → skip       │  ← different hash
  └─────────────────────────────────┘
```

### 7.2 Performance Optimization

- **Size pre-filter:** Only files with the same size are hashed.
  This eliminates the vast majority of files (most files have
  unique sizes).
- **Minimum size threshold:** Files below `DuplicateMinSizeBytes`
  are skipped entirely. Small files are rarely worth deduplicating
  and hash quickly, but in large volumes they add up.
- **Parallel hashing:** Up to 4 threads hash files concurrently
  (controlled by a `SemaphoreSlim`).
- **Streaming reads:** Files are read in 80 KB chunks to avoid
  loading large files into memory.

### 7.3 Edge Cases

- **Same size, different content:** Not grouped (different hash).
- **Empty files (0 bytes):** Excluded (trivially "duplicates" but
  useless).
- **Files in use:** `FileStream` open with `FileShare.Read` —
  files locked for write will throw and be skipped silently.
- **Permission denied:** Hash computation fails, file is skipped.

## 8. UAC Elevation

### 8.1 Implementation

`ElevationHelper` uses `ShellExecuteEx` with the `"runas"` verb
to relaunch the application with elevated rights:

```csharp
public static bool LaunchElevated(string arguments = "")
{
    var execInfo = new SHELLEXECUTEINFO
    {
        lpVerb = "runas",
        lpFile = Environment.ProcessPath,
        ...
    };
    int result = ShellExecuteEx(ref execInfo);
    return result != unchecked((int)0x800704C7); // ERROR_CANCELLED
}
```

**Key points:**
- The `app.manifest` specifies `asInvoker` — the app starts without
  elevation by default.
- The `"runas"` verb triggers the standard UAC prompt.
- If the user cancels, `ShellExecuteEx` returns `ERROR_CANCELLED`
  (0x800704C7).
- The elevated instance receives the same command-line arguments
  as the original (for passing scan state, if needed).

### 8.2 Elevation Detection

`ElevationHelper.IsElevated()` checks the process token's
`TokenIsElevated` property via `GetTokenInformation`:

```csharp
[StructLayout(LayoutKind.Sequential)]
private struct TOKEN_ELEVATION { public int TokenIsElevated; }

GetTokenInformation(tokenHandle, TokenIsElevated, ref elevation, ...);
return elevation.TokenIsElevated != 0;
```

### 8.3 Protected Path Detection

`ElevationHelper.RequiresElevation(path)` checks the path against
a list of known system-protected prefixes:
- `C:\Windows`
- `C:\Program Files`
- `C:\Program Files (x86)`
- `C:\PerfLogs`
- `C:\inetpub`

The check is case-insensitive and matches the path prefix
(including the trailing backslash to avoid false positives
like `C:\Windows.old`).

## 9. Cleanup Operations

### 9.1 Recycle Bin

Uses the Windows Shell API:

```csharp
var op = new SHFILEOPSTRUCT
{
    pFrom = path,
    fOperation = FO_DELETE,
    wFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_SILENT
};
int result = SHFileOperation(ref op);
```

- `FO_DELETE`: Delete operation.
- `FOF_ALLOWUNDO`: Send to Recycle Bin instead of permanent delete.
- `FOF_NOCONFIRMATION`: Don't show the confirmation dialog
  (we show our own).
- `FOF_SILENT`: No progress UI (we show our own status).

### 9.2 Hard Delete

```csharp
if (File.Exists(path))
    File.Delete(path);
else if (Directory.Exists(path))
    Directory.Delete(path, recursive: true);
```

- `Directory.Delete(recursive: true)` deletes the directory tree
  bottom-up.
- Errors are caught per-item — one failed deletion doesn't stop
  the batch.

### 9.3 Batch Processing

Both operations process items sequentially (to avoid overwhelming
the file system with parallel deletes). Each item is processed
independently — a failure on one item doesn't affect the others.

The `BatchCleanupResult` aggregates:
- Total items, succeeded, failed counts.
- Total bytes freed.
- Individual `CleanupResult` per item (path, success, error message).

## 10. Report Export

### 10.1 CSV

- Header row: `Full Path,Name,Extension,Size Bytes,Size Display,Depth,Is Directory,Last Write Time,File Count,Directory Count`
- Fields containing commas, quotes, or newlines are double-quoted
  (standard CSV escaping).
- UTF-8 encoding.

### 10.2 JSON

- Array of anonymous objects with all scan result properties.
- `WriteIndented = true` for readability.
- `LastWriteTime` is serialized as a string (`yyyy-MM-dd HH:mm:ss`)
  to avoid culture-dependent formatting.

### 10.3 HTML

- Standalone HTML document with embedded CSS.
- Summary section: total files, total directories, total size, timestamp.
- Table sorted by size descending.
- Directory rows are bold and blue.
- HTML escaping for all user data (paths, names).

### 10.4 TXT

- Header with summary.
- One line per entry: `[FILE]  1.23 GB  C:\path\to\file.dll`
- Directories shown as `[DIR ]` (aligned with `[FILE]`).

## 11. Configuration and Presets

### 11.1 Storage

Presets are stored as JSON in:
```
%AppData%\Win11DeTrasher\filters.json
```

```json
[
  {
    "Name": "ONNX DLLs",
    "Options": {
      "MaxDepth": -1,
      "IncludeExtensions": [".dll", ".onnx"],
      "ExcludeExtensions": [],
      "IncludePathSegments": [],
      "ExcludePathSegments": ["bin", "obj"],
      "MinFileSizeBytes": 1048576,
      "MaxFileSizeBytes": 9223372036854775807,
      "SkipHidden": true,
      "SkipSystem": false,
      "DetectDuplicates": true,
      "DuplicateMinSizeBytes": 1048576
    }
  }
]
```

### 11.2 Round-Trip

- `SavePresets` serializes the list of `ScanFilterPreset` to JSON.
- `LoadPresets` deserializes the JSON back to a list.
- If the file doesn't exist or is invalid, an empty list is returned
  (graceful degradation).

### 11.3 Thread Safety

The `ConfigService` is not thread-safe — it is only called from
the UI thread (button click handlers). This is acceptable because
the file is small and the operations are fast.

## 12. UI Architecture

### 12.1 Designer Pattern

All UI elements are defined in `MainForm.Designer.cs` (the
standard WinForms Designer pattern). Event handlers are in
`MainForm.cs`. This separation keeps the layout declarative
and the logic imperative.

### 12.2 Layout

The root layout is a one-column `TableLayoutPanel` with fixed
header/filter/progress rows and a fill-sized content row:

```
Row 0 (82px):  Drive, scan, worker and preset controls
Row 1 (285px): Filter Panel | System Info Panel (resizable split)
Row 2 (76px):  Progress bar, counts, elapsed/ETA and status
Row 3 (fill):  Results Panel | Application Log (resizable split)
```

The results table and the live log are side by side; both splitters
retain minimum panel sizes and can be adjusted by the user.

### 12.3 Custom Controls

**`SortableDataGridView`:**
- Subclasses `DataGridView`.
- Overrides column header mouse click to apply in-memory sorting.
- Three-way cycling: asc → desc → default.
- Sorts the bound `List<ScanResult>` in place via `List.Sort` with a
  reflection-based comparator (reads the column's `DataPropertyName`
  property from `ScanResult`).
- Re-binds the DataSource (set to null, then back) to trigger a
  full grid refresh.
- Default sort: size descending.

**`WorkerControl`:**
- A `UserControl` with a `NumericUpDown` and a `TrackBar`.
- Synchronized: changing one updates the other.
- `WorkerCountChanged` event fires when the value changes.
- Clamped to `[1, Environment.ProcessorCount]`.
- `IsEditable` property to enable/disable during a scan.

### 12.4 Data Binding

The results table is bound to a `BindingSource` whose `DataSource`
is set to the filtered/sorted `List<ScanResult>`. When the
DataSource changes, the grid re-binds automatically.

The `SortableDataGridView` performs in-place sorting on the list held
by the `BindingSource`, then calls `ResetBindings` to refresh the grid.

The system info grid is not data-bound — rows are added
manually (simpler for a small, fixed set of categories).

### 12.6 Application Logging

`Program` loads the `LoggerSettings` section from `appsettings.json` using
`System.Text.Json`, creates `RollingFileMemoryLogger`, which implements
`IRollingFileMemoryLogger`, and starts its rolling-file writer. Relative
`LogDirectory` values are resolved beside the executable; if the directory
cannot be created, the logger falls back to `%LOCALAPPDATA%\Win11DeTrasher\Logs`.
If the file is absent, `Program` writes it from the embedded JSON template.
The generated default keeps the latest 16 log files in LocalAppData; older
unversioned `Logs` defaults are migrated in place to that directory.
The logger source
is bundled in `Shared/RoFMLogger.cs` with its helper types and enums, so it
adds no external package dependency. `MainForm` binds `listBox_log` directly
to `RollingFileMemoryLogger.LogEntriesBindingList` and sets the WinForms
synchronization context so background messages update safely on the UI
thread. The list follows new entries while the user is at the bottom and
supports copy-selected, copy-all, and clear actions.

Scan messages include the selected root, active worker target, filter values,
completion/cancellation outcome, result counts, and sampled inaccessible-path
errors. Unexpected exceptions are logged with stack traces; repeated
access-denied/IO errors are sampled to keep the live log usable.

### 12.5 Drive ComboBox Items

The Drive ComboBox uses `DriveComboBoxItem` record instances
(a private nested record in `MainForm`) that pair a `DriveEntry`
with its display string. The record overrides `ToString()` to
return the display text, so the ComboBox shows the formatted
text (e.g. `C:\  (Local Disk — 931 GB total, 120 GB free)`)
while the underlying `Drive` property holds the actual data.
The `...` option has a null `Drive` property, which triggers
the `FolderBrowserDialog` in `GetSelectedPath()`.

## 13. Threading Model

### 13.1 Thread Roles

| Thread | Role |
|---|---|
| **UI thread** | WinForms message loop. All UI updates must happen here. |
| **Worker threads** (N) | Scan: dequeue paths, process files, enqueue subdirs. |
| **Background thread** (Task.Run) | System info collection, duplicate detection, cleanup. |
| **Progress event** | Raised on worker threads; marshaled to UI via `Invoke`. |

### 13.2 UI Thread Marshaling

All progress updates are marshaled to the UI thread:

```csharp
private void OnProgressUpdated(ScanProgress progress)
{
    this.Invoke(() =>
    {
        this._progressBar.Value = ...;
        this._elapsedLabel.Text = ...;
        // ...
    });
}
```

`Invoke` blocks the calling thread until the UI thread has
processed the delegate. This is safe because the progress
updates are throttled (max 5/second).

### 13.3 Background Operations

- **System info:** `Task.Run` — collects sizes of temp folders,
  Recycle Bin, etc. Runs on a thread pool thread.
- **Duplicate detection:** `Task.Run` with internal parallelism
  (4 threads via `SemaphoreSlim`).
- **Cleanup:** `Task.Run` — processes items sequentially.

All background operations use `CancellationToken` for cancellation
where applicable.

### 13.4 Concurrency Safety

| Scenario | How it's handled |
|---|---|
| Multiple workers accessing `_results` | `lock(_sync)` on flush |
| Multiple workers accessing `_workQueue` | `BlockingCollection` is thread-safe |
| Progress updates from multiple workers | `lock` in `ProgressTracker` |
| Worker count change during scan | `lock(_sync)` on `WorkerCount` setter |
| Scan start while scanning | `lock(_sync)` + `InvalidOperationException` |

## 14. Error Handling Strategy

### 14.1 Principle

**Never let a single file error crash the scan.** The scan is
designed to be resilient: inaccessible files are skipped and
counted as errors, but the scan continues.

### 14.2 Specific Exceptions

| Exception | Handling |
|---|---|
| `UnauthorizedAccessException` | Report error, skip file. Common for system paths without elevation. |
| `IOException` | Report error, skip file. Common for files in use. |
| `OperationCanceledException` | Clean exit. Propagate to cancel the scan. |
| Other `Exception` | Report error, skip file. Log for debugging. |

### 14.3 User-Facing Errors

- **Scan errors:** Shown in the status label and error count.
  A `MessageBox` is shown only for unexpected exceptions
  (not for per-file errors).
- **Cleanup errors:** Shown in a `MessageBox` with up to 5
  error messages.
- **Export errors:** Shown in a `MessageBox`.
- **Configuration errors:** Silent fallback (empty preset list
  if the JSON is invalid).

## 15. Testing Strategy

### 15.1 Unit Tests

All services have unit tests using a temporary directory
structure. The tests verify:
- **Happy path:** Correct behavior with valid input.
- **Filter behavior:** Each filter option works as expected.
- **Edge cases:** Empty input, non-existent paths, cancellation.
- **Thread safety:** (Implicitly tested by running the scanner
  with multiple workers.)

### 15.2 Test Isolation

- Each test creates its own temporary directory.
- Cleanup in `[TestCleanup]` removes the directory.
- Tests are independent — no shared state between tests.
- Fixed random seed (42) for reproducible test data.

### 15.3 Integration Tests (Not in Unit Suite)

The following require a real Windows environment and are
covered manually:
- Recycle Bin operations (actually deletes files).
- Hard delete (actually deletes files).
- UAC elevation (requires admin rights).
- Drive discovery (depends on installed drives).

## 16. Build and Run

### 16.1 Prerequisites

- .NET SDK 10.0 (or later)
- Windows 10/11 (64-bit)
- Visual Studio 2022 (or later) with the ".NET desktop development"
  workload, or the .NET CLI

### 16.2 Build

```bash
# From the repository root
dotnet build Win11DeTrasher.slnx -c Release

# Or build the main project only
dotnet build Win11DeTrasher/Win11DeTrasher.csproj -c Release
```

### 16.3 Run

```bash
dotnet run --project Win11DeTrasher/Win11DeTrasher.csproj -c Release -r win-x64

# Or run the built exe
Win11DeTrasher/bin/Release/net10.0-windows/win-x64/Win11DeTrasher.exe
```

### 16.4 Test

```bash
dotnet test Win11DeTrasher.Tests/Win11DeTrasher.Tests.csproj -c Release -r win-x64
```

### 16.5 Publish

```bash
dotnet publish Win11DeTrasher/Win11DeTrasher.csproj -c Release -r win-x64 \
  --self-contained true -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:EnableCompressionInSingleFile=true -o publish/
```

This produces `publish/Win11DeTrasher.exe`, a compressed, self-contained
single-file x64 application. Users do not need the .NET runtime or SDK,
and no sidecar application DLLs are required. Native libraries are bundled
and extracted to a temporary directory when the application starts.

### 16.6 Continuous Integration and GitHub Releases

`.github/workflows/release.yml` runs on every push to `master` and can
also be started manually. It builds the Release configuration, runs the
MSTest suite, publishes the self-contained single-file executable, and
attaches it to a GitHub Release. Release tags are numbered
`v1.0.<run number>` (for example, `v1.0.1`); the attached download is
`Win11DeTrasher.exe`.

## 17. Design Decisions and Trade-offs

### 17.1 Work Queue vs. Parallel.ForEach

**Decision:** Used a `BlockingCollection` work queue with a dynamic
worker pool.

**Alternative:** `Parallel.ForEach` with a fixed `ParallelOptions.MaxDegreeOfParallelism`.

**Why the work queue wins:**
- **On-the-fly worker adjustment:** The worker pool can be resized
  during a scan. `Parallel.ForEach` has a fixed degree of parallelism.
- **Natural load balancing:** Workers pick up subdirectories as
  they're discovered. No pre-computation of the work list needed.
- **Cancellation:** Clean cancellation via `CancellationToken`
  on the queue.

**Trade-off:**
- Slightly more complex than `Parallel.ForEach`.
- Requires careful management of the queue lifecycle
  (`CompleteAdding`, disposal).

### 17.2 In-Memory Results vs. Streaming to Disk

**Decision:** All results are kept in memory (`List<ScanResult>`).

**Alternative:** Stream results to a file as they're discovered.

**Why in-memory wins:**
- **Sorting:** The user can sort by any column. In-memory sorting
  is fast.
- **Search:** The user can search/filter results after the scan
  completes. Streaming to disk would require re-reading the file.
- **Duplicate detection:** Requires all file sizes and hashes
  in memory for grouping.

**Trade-off:**
- For a 900 GB drive with 5 million files, the in-memory list
  could be 2-5 GB. This is acceptable for a desktop app with
  16-64 GB of RAM.
- If the drive has 100 million+ files, memory could become
  a concern. A future optimization could cap the results list
  (e.g. keep only the top N by size) and stream the rest to disk.

### 17.3 SHA256 vs. CRC32 for Duplicate Detection

**Decision:** Used SHA256.

**Alternative:** CRC32 (faster, but weaker collision resistance).

**Why SHA256 wins:**
- **Collision resistance:** CRC32 has a 32-bit output — collisions
  are likely with millions of files. SHA256 has a 256-bit output
  — collisions are cryptographically negligible.
- **Standard:** SHA256 is the standard for content fingerprinting.
- **Performance:** The size pre-filter eliminates most files
  before hashing, so the SHA256 overhead is acceptable.

### 17.4 Hybrid UAC vs. Always Elevated

**Decision:** Hybrid — app starts unprivileged, elevation is
requested on demand.

**Alternative:** `requireAdministrator` in the manifest (always elevated).

**Why hybrid wins:**
- **Security:** The app doesn't run with admin rights unless
  needed. A compromised unprivileged app has less access.
- **UX:** No UAC prompt on every launch. The prompt only appears
  when scanning protected paths.
- **Flexibility:** The user can choose to scan without elevation
  (accepting some inaccessible files).

### 17.5 WinForms vs. WPF

**Decision:** WinForms.

**Alternative:** WPF (more modern, XAML-based).

**Why WinForms wins (for this project):**
- **User requirement:** Explicitly requested WinForms.
- **Simplicity:** WinForms is simpler for a tool with a
  straightforward layout.
- **Designer:** The WinForms Designer is mature and well-known.
- **Performance:** WinForms has lower memory overhead for
  a data-heavy UI (large DataGrid).

## 18. Performance Characteristics

### 18.1 Scan Throughput

On a modern NVMe SSD with 16 cores:
- **5 million files:** ~2-5 minutes (with 16 workers)
- **1 million files:** ~30-60 seconds
- **100,000 files:** ~5-15 seconds

On a 7200 RPM HDD:
- **5 million files:** ~10-30 minutes
- **1 million files:** ~2-5 minutes

The throughput is limited by:
1. **Disk I/O:** Random access to file metadata (stat calls).
2. **Directory enumeration:** `Directory.GetFiles`/`GetDirectories`
   is slow for large directories.
3. **Lock contention:** The shared results list lock.

### 18.2 Duplicate Detection

- **10,000 candidate files (after size pre-filter):** ~5-15 seconds
  (4 threads, 80 KB buffer).
- **100,000 candidate files:** ~2-5 minutes.
- **1 million candidate files:** ~15-30 minutes.

The size pre-filter is the key optimization — for typical
file sets, only 5-20% of files have duplicate sizes.

### 18.3 Memory Usage

| Component | Memory |
|---|---|
| Results list (5M files) | ~2-5 GB |
| Progress state | Negligible |
| Work queue (bounded) | ~1-2 MB |
| Duplicate detection (size groups) | ~500 MB - 1 GB |
| UI (DataGridView with 5M rows) | ~1-2 GB (virtualized) |
| **Total (peak)** | ~4-8 GB |

> For a 16 GB RAM system, this is comfortable. For an 8 GB
> RAM system, consider using the min-size filter to reduce
> the number of results.

### 18.4 CPU Usage

- **Scan:** CPU usage is proportional to the worker count.
  With 16 workers on a 16-core CPU, expect ~100% CPU usage
  during the scan (I/O-bound, so actual CPU usage will be lower).
- **Duplicate detection:** 4 threads hashing — moderate CPU usage.
- **UI:** Minimal CPU usage (progress updates are throttled).

---

## 19. Appendix — API Reference

### 19.1 Service Interfaces

| Interface | Key Methods | Description |
|---|---|---|
| `IFileScanner` | `ScanAsync(rootPath, filterOptions, ct)`, `WorkerCount {get;set;}`, `IsScanning` | Concurrent recursive scan with on-the-fly worker adjustment |
| `IProgressTracker` | `Start()`, `ReportDiscovered(count, bytes)`, `ReportProcessed(count, bytes)`, `ReportError()`, `Complete()`, `Cancel()`, `Reset()`, `ProgressUpdated` event | Thread-safe progress tracking with throttled events |
| `IDuplicationDetector` | `DetectDuplicatesAsync(results, minSizeBytes, ct)` | SHA256-based duplicate detection with size pre-filter |
| `ISystemInfoService` | `GetSystemInfoAsync(ct)`, `GetLargestFilesAsync(root, topN, ct)`, `GetFileAgeStatsAsync(root, ct)` | System info collection (temp folders, Recycle Bin, etc.) |
| `IReportExporter` | `ExportCsv()`, `ExportJson()`, `ExportHtml()`, `ExportTxt()` | Export scan results to 4 file formats |
| `ICleanupService` | `SendToRecycleBinAsync(paths, ct)`, `HardDeleteAsync(paths, ct)`, `GetTotalSize(paths)` | Recycle Bin (reversible) and Hard Delete (permanent) |
| `IConfigService` | `LoadPresets()`, `SavePresets(presets)`, `PresetsFilePath` | JSON-based filter preset persistence |
| `IDriveDiscoveryService` | `GetAvailableDrivesAsync()` | Enumerate system drives with size info |

### 19.2 Model Types

| Type | Kind | Description |
|---|---|---|
| `ScanResult` | class | A single file or directory entry with all metadata |
| `ScanFilterOptions` | class | Serializable filter configuration for a scan |
| `ScanProgress` | readonly record struct | Immutable progress snapshot (thread-safe publication) |
| `SystemInfoEntry` | record | A system info category with path and size |
| `DuplicateGroup` | record | A group of files with identical SHA256 hashes |
| `CleanupResult` | record | Per-item cleanup outcome (path, success, error, bytes freed) |
| `BatchCleanupResult` | record | Aggregated cleanup result (counts, total freed, per-item results) |
| `ScanFilterPreset` | record | A named filter configuration for JSON persistence |
| `DriveEntry` | record | A drive letter, name, type, and size info |

### 19.3 Utility Classes

| Class | Methods | Description |
|---|---|---|
| `SizeFormatter` | `Format(bytes)`, `TryParse(text, out bytes)` | Byte ↔ human-readable string conversion |
| `ElevationHelper` | `IsElevated()`, `LaunchElevated(args)`, `RequiresElevation(path)` | UAC elevation check, launch, and protected-path detection |

### 19.4 Custom Controls

| Control | Base Class | Key Properties/Events | Description |
|---|---|---|---|
| `SortableDataGridView` | `DataGridView` | `ApplySort(column)`, `ResetSort()`, `SortStateChanged` event, `CurrentSortDirection` | Three-way sort cycling on column header click |
| `WorkerControl` | `UserControl` | `WorkerCount {get;set;}`, `MaxWorkers`, `IsEditable`, `WorkerCountChanged` event | NumericUpDown + TrackBar for worker count adjustment |

### 19.5 P/Invoke Signatures

| Function | DLL | Purpose |
|---|---|---|
| `ShellExecuteEx` | shell32.dll | Relaunch app with elevated rights (`"runas"` verb) |
| `SHFileOperation` | shell32.dll | Send files to Recycle Bin (`FO_DELETE` + `FOF_ALLOWUNDO`) |
| `SHQueryRecycleBin` | shell32.dll | Query Recycle Bin size and item count |
| `OpenProcessToken` | advapi32.dll | Open the current process token for elevation check |
| `GetTokenInformation` | advapi32.dll | Query token elevation level (class 12) |
| `CloseHandle` | kernel32.dll | Close the token handle after querying |

---

*End of Technical Documentation*
