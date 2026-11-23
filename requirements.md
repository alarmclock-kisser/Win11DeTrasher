# Win11DeTrasher — Requirements

> **Status:** Living document. Updated as requirements evolve.
> **Last updated:** 2026-11-23

---

## Table of Contents

1. [Overview](#1-overview)
2. [Target Platform](#2-target-platform)
3. [Core Functionality](#3-core-functionality)
4. [User Interface](#4-user-interface)
5. [Scan Engine](#5-scan-engine)
6. [Filters and Presets](#6-filters-and-presets)
7. [Progress Reporting](#7-progress-reporting)
8. [Results Display](#8-results-display)
9. [System Information](#9-system-information)
10. [Duplicate Detection](#10-duplicate-detection)
11. [Cleanup Operations](#11-cleanup-operations)
12. [Report Export](#12-report-export)
13. [UAC Elevation](#13-uac-elevation)
14. [Architecture and Code Quality](#14-architecture-and-code-quality)
15. [Testing](#15-testing)
16. [Documentation](#16-documentation)
17. [Out of Scope / Future Ideas](#17-out-of-scope--future-ideas)

---

## 1. Overview

Win11DeTrasher is a Windows desktop application that helps the user analyze
disk usage on a drive (primarily the C: drive), identify large files,
directories, duplicates, and temporary data, and perform cleanup operations
(send to Recycle Bin or hard delete). The goal is to provide an interactive,
informative, and safe tool for reclaiming disk space.

**Primary use case:** The user's C: drive is 900+ GB and nearly full.
They need to find and remove the data structures and files that are
consuming the most space, including NuGet caches, ONNX binaries in
source repositories, Windows temp files, and other space hogs.

## 2. Target Platform

| Property | Value |
|---|---|
| Framework | .NET 10.0 (`net10.0-windows`) |
| Runtime identifier | `win-x64` |
| UI technology | Windows Forms (WinForms) |
| OS | Windows 11 (also compatible with Windows 10) |
| Architecture | 64-bit (x64) |
| Distribution | Compressed, self-contained single-file executable; no .NET runtime/SDK required |

## 3. Core Functionality

- **Recursive scan:** Given a drive or path, recursively enumerate all
  files and directories, computing sizes, depths, and metadata.
- **Drive selection:** ComboBox listing all available drives with
  total/free space. Option to browse to a custom path.
- **Progress display:** Real-time progress bar, elapsed time, ETA,
  file counts, and error count during the scan.
- **Results table:** Sortable table of all discovered files and
  directories, with columns for path, name, extension, size, depth,
  type, last write time, and duplicate group.
- **Cleanup:** Send selected items to the Recycle Bin (reversible)
  or hard delete (permanent, with confirmation).
- **Report export:** Export scan results to CSV, JSON, HTML, or TXT.

## 4. User Interface

### 4.1 General

- **Language:** All user-facing strings (labels, buttons, messages,
  tooltips) in **English**. Code comments in English.
- **Layout:** Responsive, dynamically resizable window.
  Uses a `TableLayoutPanel` for the main layout.
- **Window properties:**
  - Minimum size: 800×600
  - Default size: 1100×800
  - Maximizable: Yes
  - DPI awareness: PerMonitorV2 (via `app.manifest`)
  - Start position: CenterScreen
- **UI elements in Designer:** All UI elements are defined in
  `MainForm.Designer.cs` (not created at runtime in code-behind).
  Event handlers are in `MainForm.cs`.

### 4.2 Layout Structure

```
+----------------------------------------------------------+
| Top Panel (60px)                                        |
| [Drive:] [ComboBox] [Scan] [Cancel] [WorkerControl]     |
| [Preset:] [ComboBox] [Save Preset] [Delete Preset]      |
+----------------------------------------------------------+
| Filter Panel (30%)           | System Info Panel (40%)   |
| Include/Exclude extensions   | [Refresh System Info]     |
| Include/Exclude paths        | [DataGridView]           |
| Max depth, Min/Max size      |   Category | Path | Size |
| Skip hidden/system           |   Description              |
| Detect duplicates + min size |                          |
+----------------------------------------------------------+
| Progress Panel (80px)                                   |
| [ProgressBar]                                           |
| [Elapsed] [ETA] [Files] [Errors] [Status]               |
+----------------------------------------------------------+
| Results Panel (70%)                                    |
| [Search:] [TextBox] [Min size:] [NumericUpDown]         |
| [SortableDataGridView]                                  |
| [Export Report...] [Send to Recycle Bin] [Hard Delete]  |
+----------------------------------------------------------+
```

### 4.3 Controls

- **Drive ComboBox:** Lists drives as `C:\  (Local Disk — 931 GB total, 120 GB free)`.
  Each item is a `DriveComboBoxItem` record (wraps `DriveEntry` + display string,
  overrides `ToString()`). Includes a `...  (browse to a specific folder)` option
  for custom path browsing via `FolderBrowserDialog`.
- **Scan / Cancel buttons:** Scan starts the scan; Cancel is enabled only
  during a running scan.
- **WorkerControl:** Custom `UserControl` with a `NumericUpDown` and a
  `TrackBar` for adjusting the number of scanner worker threads
  (1 to `Environment.ProcessorCount`). Adjustable on-the-fly during a scan.
- **Filter inputs:** TextBoxes for include/exclude extensions and path
  segments (comma-separated). NumericUpDowns for max depth, min/max file size.
  CheckBoxes for skip hidden/system, detect duplicates.
- **ProgressBar:** Standard WinForms `ProgressBar` (0-100%).
- **Status labels:** Elapsed time, ETA, file count, error count, status text.
- **Search TextBox:** Filters results by name or path (case-insensitive).
- **Min Size NumericUpDown:** Filters results by minimum file size in bytes.
- **SortableDataGridView:** Custom `DataGridView` subclass with three-way
  column-header sort cycling (ascending → descending → default).
- **System Info DataGridView:** Read-only grid showing temp folder sizes,
  Recycle Bin size, and other system info categories.
- **Cleanup buttons:** "Send to Recycle Bin" and "Hard Delete (final)" —
  both operate on selected rows in the results table.
- **Export button:** Opens a `SaveFileDialog` with format selection
  (CSV, JSON, HTML, TXT).

## 5. Scan Engine

### 5.1 Architecture

- **Work queue pattern:** A `BlockingCollection<string>` serves as a work
  queue. The root path is enqueued initially. Workers dequeue paths,
  process them, and enqueue subdirectories for further processing.
- **Worker pool:** A configurable number of worker threads (default: 4,
  max: `Environment.ProcessorCount`) consume from the work queue in parallel.
- **On-the-fly worker adjustment:** Changing `WorkerCount` during a running
  scan starts additional workers (if count increases). When the count
  decreases, excess workers naturally drain the queue and exit without
  forced termination (no data loss).
- **Cancellation:** A `CancellationTokenSource` is linked to the external
  cancellation token. Canceling stops all workers cleanly.
- **Thread safety:** All shared state (results list, progress counters)
  is protected by a `lock`. Progress events are throttled to at most
  5 per second to avoid flooding the UI thread.

### 5.2 Per-File Processing

For each file or directory dequeued from the work queue:
1. Get file attributes.
2. Check hidden/system skip filters.
3. Check depth limit (for directories).
4. Check size filters (min/max).
5. Check extension include/exclude filters.
6. Check path segment include/exclude filters.
7. Create a `ScanResult` with all metadata.
8. For directories: collect subdirectory paths for enqueueing.

### 5.3 Error Handling

- `UnauthorizedAccessException`: Report error, skip the file.
- `IOException`: Report error, skip the file.
- Other exceptions: Report error, skip the file.
- The scan continues despite individual file errors.

## 6. Filters and Presets

### 6.1 Filter Options

| Filter | Type | Description |
|---|---|---|
| `MaxDepth` | int (-1 = unlimited) | Maximum scan depth relative to root |
| `IncludeExtensions` | List<string> | Only include files with these extensions (case-insensitive) |
| `ExcludeExtensions` | List<string> | Exclude files with these extensions (case-insensitive) |
| `IncludePathSegments` | List<string> | Only include files whose relative path contains one of these segments |
| `ExcludePathSegments` | List<string> | Exclude files whose relative path contains any of these segments |
| `MinFileSizeBytes` | long (0 = no limit) | Minimum file size to include |
| `MaxFileSizeBytes` | long (max = no limit) | Maximum file size to include |
| `SkipHidden` | bool | Skip files/directories with the Windows hidden attribute |
| `SkipSystem` | bool | Skip files/directories with the Windows system attribute |
| `DetectDuplicates` | bool | Enable SHA256-based duplicate detection |
| `DuplicateMinSizeBytes` | long | Minimum file size for duplicate detection (default: 1 MB) |

### 6.2 Presets

- Presets are saved as JSON in `%AppData%/Win11DeTrasher/filters.json`.
- Each preset has a `Name` and `ScanFilterOptions`.
- The UI has a preset ComboBox, "Save Preset" button (prompts for name),
  and "Delete Preset" button.
- Selecting a preset populates all filter fields.
- Presets can be saved at any time (even during a scan).

## 7. Progress Reporting

### 7.1 Real-Time Updates

- The `ProgressTracker` publishes `ScanProgress` snapshots via an event.
- The UI subscribes and marshals updates to the UI thread via `Invoke`.
- Events are throttled to at most every 200ms (except on completion/cancellation).
- The progress bar shows percentage (0-100%), calculated as
  `processedBytes / totalBytes * 100` (with fallback to file counts).

### 7.2 Displayed Metrics

- **Elapsed time:** Formatted as `Xh Ym Zs` or `Ym Zs` or `Z.Zs`.
- **ETA (Estimated Time Remaining):** Computed as
  `(totalBytes - processedBytes) / (processedBytes / elapsedSeconds)`.
  Shown as "ETA: calculating..." until at least 1 second has elapsed
  and progress > 0.
- **File count:** `processedFiles / totalFiles`.
- **Error count:** Number of files that could not be accessed.
- **Status text:** "Ready.", "Scanning...", "Scan complete.", "Scan cancelled.",
  "Detecting duplicates...", etc.

## 8. Results Display

### 8.1 Table Columns

| Column | Data Property | Description |
|---|---|---|
| Full Path | `FullPath` | The full file system path |
| Name | `Name` | File or directory name |
| Extension | `Extension` | File extension (empty for directories) |
| Size | `SizeDisplay` | Human-readable size (e.g. "1.23 GB") |
| Size (bytes) | `SizeBytes` | Raw byte count (for sorting) |
| Depth | `Depth` | Depth relative to scan root |
| Type | `IsDirectory` | "DIR" or "FILE" |
| Last Write | `LastWriteTime` | Last modification timestamp |
| Duplicate | `IsDuplicate` | "YES" or empty (if in a duplicate group) |
| Hash | `Hash` | SHA256 hash (if computed) |

### 8.2 Sorting

- Click a column header to sort by that column.
- Three-way cycling: **ascending → descending → default order** (original).
- A "Reset Sort" mechanism is available (clicking the last-sorted column
  a third time returns to default order).
- Default order: size descending (largest first).
- Implementation: `SortableDataGridView` sorts the bound `List<ScanResult>`
  in place via `List.Sort` with a reflection-based comparator, then
  re-binds the DataSource to trigger a grid refresh.

### 8.3 Filtering

- **Search box:** Case-insensitive filter on file name or full path.
- **Min size filter:** NumericUpDown in bytes; filters out files below
  the threshold (directories are not filtered by size).

## 9. System Information

### 9.1 Categories

The `SystemInfoService` collects sizes for:

| Category | Path | Description |
|---|---|---|
| Windows Temp | `C:\Windows\Temp` | System-wide temporary files |
| User Temp | `%TEMP%` | Current user's temporary files |
| Prefetch | `C:\Windows\Prefetch` | Application launch optimization cache |
| Recycle Bin | (via SHQueryRecycleBin) | Deleted files waiting for permanent removal |
| WinSxS | `C:\Windows\WinSxS` | Windows component store |
| Service Profiles | `C:\Windows\ServiceProfiles` | Windows service profiles |
| ProgramData | `%ProgramData%` | System-wide application data |

### 9.2 Display

- Shown in a read-only `DataGridView` with columns: Category, Path,
  Size, Description.
- A "Refresh System Info" button re-queries all categories.
- A "Total" row at the bottom sums all category sizes.

## 10. Duplicate Detection

### 10.1 Algorithm

1. **Phase 1 — Group by size:** Filter out directories and files below
   the `DuplicateMinSizeBytes` threshold. Group remaining files by
   their `SizeBytes` value.
2. **Phase 2 — Hash candidates:** Only groups with 2+ files are candidates.
   Compute SHA256 hashes for all candidate files in parallel
   (up to 4 threads, 80 KB read buffer per file).
3. **Phase 3 — Group by hash:** Group files with identical SHA256 hashes.
   Only groups with 2+ files are reported.
4. **Sort:** Results are sorted by potential savings (largest first).

### 10.2 Performance Considerations

- Only files with the same size are hashed (avoids hashing all files).
- Hashing is parallelized across 4 threads.
- Files below the `DuplicateMinSizeBytes` threshold are skipped entirely
  (default: 1 MB — small files are rarely worth deduplicating).
- The feature is opt-in via a checkbox in the UI.

## 11. Cleanup Operations

### 11.1 Send to Recycle Bin

- Uses the Windows Shell API (`SHFileOperation` with `FO_DELETE` +
  `FOF_ALLOWUNDO` + `FOF_NOCONFIRMATION` + `FOF_SILENT`).
- Operation is **reversible** — items can be restored from the Recycle Bin.
- Confirmation dialog shows the count of items and total size.
- Partial failures are reported (up to 5 error messages shown).

### 11.2 Hard Delete

- Uses `File.Delete` for files and `Directory.Delete(recursive: true)`
  for directories.
- Operation is **permanent** — items cannot be recovered.
- Two-stage confirmation:
  1. First dialog: "PERMANENTLY DELETE N items? Total size: X. This
     action CANNOT be undone."
  2. If total size > 100 MB: second dialog "You are about to permanently
     delete more than 100 MB. Are you absolutely sure?"
- Partial failures are reported (up to 5 error messages shown).

### 11.3 Common Behavior

- Both operations run on a background thread (`Task.Run`).
- The status label is updated with the result (succeeded/failed count,
  total bytes freed).
- After cleanup, the results table is refreshed.

## 12. Report Export

### 12.1 Formats

| Format | Description |
|---|---|
| CSV | Standard comma-separated values with header row. Quoting for fields containing commas/quotes. |
| JSON | Array of objects with all scan result properties. Pretty-printed. |
| HTML | Standalone HTML document with styled table, summary section, and browser-compatible CSS. |
| TXT | Plain text with summary header and one line per result. |

### 12.2 Export Dialog

- `SaveFileDialog` with filter: "All supported formats (*.csv;*.json;*.html;*.txt)".
- Default filename: `Win11DeTrasher_Report_yyyyMMdd_HHmmss`.
- The format is inferred from the file extension.

## 13. UAC Elevation

### 13.1 Strategy

- The application runs with `asInvoker` (via `app.manifest`) by default.
- When the user selects a path that requires elevation (e.g. `C:\Windows`,
  `C:\Program Files`), a dialog appears:
  > "This path may require elevated (UAC) permissions to scan.
  > Would you like to re-run Win11DeTrasher as Administrator?
  > Note: The current scan results will be lost."
- If the user confirms, the application relaunches itself via
  `ShellExecuteEx` with the `"runas"` verb.
- If the user cancels the UAC prompt or declines, the scan proceeds
  without elevation (some files may be inaccessible).
- `ElevationHelper.IsElevated()` checks the current process token
  elevation level.
- `ElevationHelper.RequiresElevation(path)` checks the path against
  known system-protected prefixes.

## 14. Architecture and Code Quality

### 14.1 SOLID Compliance

| Principle | Application |
|---|---|
| **S**ingle Responsibility | Each class has one reason to change: `FileScanner` scans, `ProgressTracker` tracks progress, `DuplicationDetector` detects duplicates, `ReportExporter` exports, `CleanupService` deletes, `ConfigService` persists presets, `SystemInfoService` collects system info, `DriveDiscoveryService` lists drives. |
| **O**pen/Closed | All services are behind interfaces (`IFileScanner`, `IProgressTracker`, etc.), allowing new implementations without modifying existing code. |
| **L**iskov Substitution | All interface implementations can be substituted for the interface without breaking the client. |
| **I**nterface Segregation | Interfaces are focused: `IDriveDiscoveryService` has one method, `IReportExporter` has four export methods, etc. |
| **D**ependency Inversion | `MainForm` depends on abstractions (interfaces), not concrete classes. Dependencies are injected via the constructor. |

### 14.2 Code Conventions

- **Naming:** PascalCase for types, methods, properties. camelCase for
  local variables, parameters. `_camelCase` for private fields.
- **Comments:** All public members have XML doc comments in English.
  Code comments are in English.
- **String formatting:** All user-visible strings in English.
- **Null safety:** Nullable reference types enabled. All nullable
  reference types are annotated with `?`.
- **Async:** All I/O-bound operations are async (`async/await`).
  UI thread marshaling uses `Invoke`.
- **Concurrency:** `BlockingCollection` for the work queue, `lock` for
  shared state, `CancellationToken` for cancellation, `SemaphoreSlim`
  for limiting parallelism in duplicate detection.
- **Error handling:** Specific exceptions caught (`UnauthorizedAccessException`,
  `IOException`) with error reporting. Generic `catch` blocks are used
  only as a last resort with logging.

### 14.3 Project Structure

```
Win11DeTrasher/
├── Win11DeTrasher.slnx
├── .gitignore
├── requirements.md
├── user_manual.md
├── technical_documentation.md
├── Win11DeTrasher/
│   ├── Win11DeTrasher.csproj
│   ├── app.manifest
│   ├── Program.cs
│   ├── MainForm.cs
│   ├── MainForm.Designer.cs
│   ├── Controls/
│   │   ├── SortableDataGridView.cs
│   │   └── WorkerControl.cs
│   ├── UI/
│   │   └── ScanFilterForm.cs
│   ├── Services/
│   │   ├── IDriveDiscoveryService.cs
│   │   ├── DriveDiscoveryService.cs
│   │   ├── IFileScanner.cs
│   │   ├── FileScanner.cs
│   │   ├── IProgressTracker.cs
│   │   ├── ProgressTracker.cs
│   │   ├── IDuplicationDetector.cs
│   │   ├── DuplicationDetector.cs
│   │   ├── ISystemInfoService.cs
│   │   ├── SystemInfoService.cs
│   │   ├── IReportExporter.cs
│   │   ├── ReportExporter.cs
│   │   ├── ICleanupService.cs
│   │   ├── CleanupService.cs
│   │   ├── IConfigService.cs
│   │   └── ConfigService.cs
│   ├── Models/
│   │   ├── ScanResult.cs
│   │   ├── ScanFilterOptions.cs
│   │   ├── ScanProgress.cs
│   │   ├── SystemInfoEntry.cs
│   │   ├── DuplicateGroup.cs
│   │   ├── CleanupResult.cs
│   │   └── ScanFilterPreset.cs
│   └── Utils/
│       ├── ElevationHelper.cs
│       └── SizeFormatter.cs
└── Win11DeTrasher.Tests/
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

## 15. Testing

### 15.1 Framework

- **MSTest** (v4.3.2) for test structure and assertions.
- **Shouldly** (v4.3.0) for fluent assertions.
- **Moq** (v4.20.72) for mocking (available for future use).

### 15.2 Test Coverage

| Test Class | What it tests |
|---|---|
| `SizeFormatterTests` | Byte-to-string formatting and string-to-byte parsing |
| `ProgressTrackerTests` | Start/stop, counter increments, progress percentage, ETA, completion/cancellation, reset, event raising |
| `FileScannerTests` | Full scan on temp directory, extension filters, size filters, depth limits, cancellation, worker count clamping, progress reporting, concurrent scan prevention |
| `DuplicationDetectorTests` | Duplicate group detection, same-size-different-content exclusion, min size filter, no-duplicates case, cancellation |
| `ReportExporterTests` | CSV/JSON/HTML/TXT export, CSV escaping, empty results, invalid path |
| `ConfigServiceTests` | Preset save/load round-trip, missing file handling, path location |
| `DriveDiscoveryServiceTests` | Drive enumeration, C: drive presence, drive type validation, total ≥ free |
| `CleanupServiceTests` | Total size calculation for files, directories, non-existent paths, empty lists |

### 15.3 Test Conventions

- Tests inherit from `TestBase` for common utilities (temp directory
  creation, file creation, directory structure creation).
- Each test creates its own temporary directory and cleans up in
  `[TestCleanup]`.
- Tests are independent and do not depend on execution order.
- Integration tests (Recycle Bin, Hard Delete, UAC elevation) are
  not included in the unit test suite — they require a real Windows
  environment and are covered manually.

### 15.4 Known Limitations

- **ConfigService tests modify real AppData:** The `ConfigServiceTests`
  save/load presets in the actual `%AppData%` directory. They back up
  and restore the file, but this is a side effect that should be
  addressed in a future version by injecting a path provider.
- **In-memory result storage:** All scan results are kept in memory.
  For a 900 GB drive with 5 million files, this uses roughly 2–5 GB
  of RAM. For drives with 100 million+ files, memory could become
  a concern. A future optimization could cap the results list
  (e.g. keep only the top N by size) and stream the rest to disk.

## 16. Documentation

Three documentation files are included in the repository root:

1. **`user_manual.md`** — A user-facing guide explaining how to use
   the application, from launch to cleanup.
2. **`technical_documentation.md`** — A developer-facing document
   explaining the architecture, design decisions, and implementation
   details.
3. **`requirements.md`** — This file. The living requirements
   specification.

## 17. Out of Scope / Future Ideas

The following features are **not** in the current scope but may be
considered for future versions:

- **Cloud integration:** Upload large files to cloud storage before
  deleting.
- **Scheduled scans:** Automatically scan and report on a schedule.
- **Multi-drive comparison:** Compare sizes across multiple drives.
- **File type aggregation:** Group results by extension and show
  total size per extension (e.g. all .dll files total 15 GB).
- **File age histogram:** Visual chart of file age distribution.
- **Integration with Windows Storage Sense:** Trigger Windows' built-in
  cleanup via the UI.
- **Command-line interface:** A CLI version for scripting.
- **Localization:** Additional language support (currently English only).
- **File preview:** Preview file content before deleting.
- **Undo/Redo:** Undo cleanup operations (only Recycle Bin supports restore).
