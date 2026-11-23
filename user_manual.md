# Win11DeTrasher — User Manual

> **Version:** 1.0
> **Last updated:** 2026-11-23

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Getting Started](#2-getting-started)
3. [The Main Window](#3-the-main-window)
4. [Scanning a Drive](#4-scanning-a-drive)
5. [Configuring Filters](#5-configuring-filters)
6. [Understanding the Results](#6-understanding-the-results)
7. [Searching and Filtering Results](#7-searching-and-filtering-results)
8. [System Information Panel](#8-system-information-panel)
9. [Duplicate Detection](#9-duplicate-detection)
10. [Cleaning Up: Recycle Bin vs. Hard Delete](#10-cleaning-up-recycle-bin-vs-hard-delete)
11. [Exporting Reports](#11-exporting-reports)
12. [Saving Filter Presets](#12-saving-filter-presets)
13. [UAC Elevation](#13-uac-elevation)
14. [Tips for C: Drive Cleanup](#14-tips-for-c-drive-cleanup)
15. [Troubleshooting](#15-troubleshooting)
16. [Glossary](#16-glossary)
17. [Footnotes — Technical Details](#17-footnotes--technical-details)

---

## 1. Introduction

Win11DeTrasher is a disk space analyzer and cleaner for Windows.
It helps you find the files and directories that are taking up the most
space on your drives, and safely remove them.

**What it can do:**

- Scan any drive or folder recursively
- Show a sortable table of all files and directories with sizes
- Detect duplicate files (by content hash)
- Show system info: temp folders, Recycle Bin, WinSxS, and more
- Send files to the Recycle Bin (reversible) or hard delete (permanent)
- Export scan results to CSV, JSON, HTML, or TXT

**Who is it for:**

Anyone whose C: drive is running low on space and needs to find
what's eating the disk. Particularly useful for developers with
large NuGet caches, ONNX binaries, or multiple source repositories
with gigabytes of model files.

## 2. Getting Started

1. **Launch the application:** Run `Win11DeTrasher.exe` (double-click
   or from the Start menu).
2. **Wait for the main window to appear:** The window will show
   a list of available drives and an empty results table.
3. **Select a drive:** Click the Drive ComboBox and choose the
   drive you want to scan (e.g. `C:\`).
4. **Click "Scan":** The scan will start and progress will be
   displayed in real time.

That's it. The rest of this manual explains all the features in detail.

## 3. The Main Window

The main window has five main sections, from top to bottom:

| Section | What it does |
|---|---|
| **Top Panel** | Drive selection, scan/cancel buttons, worker count, preset management |
| **Filter Panel** | Configure which files to include/exclude in the scan |
| **Progress Panel** | Real-time scan progress: bar, elapsed time, ETA, file counts |
| **Results Panel** | Sortable table of scan results, search/filter, cleanup actions |
| **System Info Panel** | (right side) Sizes of temp folders, Recycle Bin, etc. |

The window is **dynamically resizable** — you can drag the edges to
make it larger or smaller, or maximize it to fill the screen.
All elements resize proportionally.

### 3.1 The Top Panel

```
[Drive:] [C:\  (Local Disk — 931 GB total, 120 GB free)  v]
[Preset:] [My Preset  v] [Save Preset] [Delete Preset]

[Scan] [Cancel]
[Workers: 4  [====|=====]  (4/16 cores)]
```

- **Drive ComboBox:** Shows all available drives with their type,
  total size, and free space. Each entry looks like
  `C:\  (Local Disk — 931 GB total, 120 GB free)`.
  Select the drive you want to scan.
  Choose `...  (browse to a specific folder)` to open a folder browser.
- **Scan button:** Starts the scan. Disabled while a scan is running.
- **Cancel button:** Stops the running scan. Enabled only during a scan.
- **WorkerControl:** Adjusts the number of scanner threads.
  - Use the **NumericUpDown** (the small number box) for precise values.
  - Use the **slider** for quick adjustment.
  - The value is clamped to `1` (minimum) to `Environment.ProcessorCount`
    (maximum, e.g. 16 on a 16-core CPU).
  - **You can change this while a scan is running** — the scanner
    will adapt without stopping. If the system feels slow, reduce
    the worker count. If you want maximum speed, increase it.
- **Preset ComboBox:** Select a saved filter preset to pre-fill the
  filter settings. See [Section 12](#12-saving-filter-presets) for details.
- **Save Preset / Delete Preset:** Manage your saved filter presets.

### 3.2 The Filter Panel

The filter panel has six rows of controls:

| Row | Controls | Purpose |
|---|---|---|
| 1 | **Include extensions** (TextBox) | Only scan files with these extensions (e.g. `.dll, .onnx`) |
| 2 | **Exclude extensions** (TextBox) | Skip files with these extensions (e.g. `.tmp, .log`) |
| 3 | **Include paths containing** (TextBox) | Only scan files whose path contains one of these segments |
| 4 | **Exclude paths containing** (TextBox) | Skip files whose path contains any of these segments |
| 5 | **Max depth** (NumericUpDown) | How deep to scan (0 = only the root, -1 = unlimited) |
| 6 | **Min size / Max size** (NumericUpDowns) | Only include files within this size range (in bytes) |
| 7 | **Skip hidden / Skip system** (CheckBoxes) | Filter out Windows hidden/system files |
| 8 | **Detect duplicates** (CheckBox) + **Min size for duplicates** (NumericUpDown) | Enable SHA256 duplicate detection |

> **Tip:** Leave all fields empty to scan everything. The filters
> are cumulative — a file must pass **all** active filters to be included.

### 3.3 The Progress Panel

```
[============================----------]  65%
Elapsed: 3m 42s    ETA: 2m 10s    Files: 12,345 / 18,900    Errors: 23
Scanning...
```

- **Progress bar:** Shows the percentage of the scan completed.
- **Elapsed:** How long the scan has been running.
- **ETA:** Estimated time remaining. Shown as "calculating..." until
  the scanner has enough data to compute a reliable estimate
  (at least 1 second of elapsed time and some progress).
- **Files:** Number of files processed so far / total files discovered.
- **Errors:** Number of files that could not be accessed (permission
  denied, in use, etc.). These are skipped but counted.
- **Status:** Text description of the current state ("Scanning...",
  "Detecting duplicates...", "Scan complete.", etc.).

### 3.4 The Results Panel

```
[Search:] [_______________]  [Min size:] [_______]

+------------------------------------------+
| Path          | Name | Ext | Size  | ... |
+------------------------------------------+
| C:\...      | file | .dll| 5.2 GB|     |
| C:\...      | file | .txt| 1.1 GB|     |
| ...         | ...  | ... | ...   |     |
+------------------------------------------+

[Export Report...] [Send to Recycle Bin] [Hard Delete (final)]
```

- **Search box:** Type to filter results by file name or path
  (case-insensitive). E.g. type `dll` to show only .dll files.
- **Min size:** Set a minimum file size in bytes to filter out
  small files. E.g. set `1048576` (1 MB) to hide files smaller
  than 1 MB.
- **Results table:** All files and directories found during the scan.
  Click any column header to sort (see [Section 6](#6-understanding-the-results)).
- **Export Report:** Save the results to a file (CSV, JSON, HTML, TXT).
- **Send to Recycle Bin:** Move selected items to the Recycle Bin.
- **Hard Delete:** Permanently delete selected items (with confirmation).

### 3.5 The System Info Panel (right side)

```
[Refresh System Info]
+----------------+----------------------+-----------+
| Category       | Path                 | Size      |
+----------------+----------------------+-----------+
| Windows Temp   | C:\Windows\Temp      | 2.3 GB    |
| User Temp      | C:\Users\...\Temp    | 1.1 GB    |
| Recycle Bin    | (N/A)                | 5.7 GB    |
| WinSxS         | C:\Windows\WinSxS    | 12.4 GB   |
| ...            | ...                  | ...       |
| Total          |                      | 25.1 GB   |
+----------------+----------------------+-----------+
```

Shows the sizes of well-known space-hungry locations on your system.
Click **Refresh System Info** to re-query all categories.

## 4. Scanning a Drive

### 4.1 Basic Scan

1. Select a drive from the ComboBox (e.g. `C:\`).
2. Click **Scan**.
3. Watch the progress bar and status updates.
4. When the scan completes, the results table is populated.

A full scan of a 900 GB C: drive with 3-5 million files typically
takes **2-10 minutes** depending on your hardware (SSD vs. HDD),
the number of files, and the worker count.

### 4.2 Scanning a Specific Folder

1. Select `...  (browse to a specific folder)` from the Drive ComboBox.
2. In the folder browser dialog, navigate to the folder you want to scan.
3. Click **OK**, then **Scan**.

### 4.3 Scanning with Filters

Configure the filters in the Filter Panel before clicking Scan.
See [Section 5](#5-configuring-filters) for details.

### 4.4 Stopping a Scan

Click **Cancel** at any time. The scan will stop, and any results
collected up to that point will be displayed in the results table.

### 4.5 Adjusting Workers During a Scan

If the scan is making your system feel slow (high disk/CPU usage),
you can reduce the worker count **without stopping the scan**:

1. Look at the WorkerControl in the top panel.
2. Lower the number (e.g. from 16 to 4) using the NumericUpDown or slider.
3. The scanner will adapt — new work will be distributed among fewer threads.

Conversely, if the scan is too slow and your system has headroom,
increase the worker count.

## 5. Configuring Filters

### 5.1 Extension Filters

**Include extensions** — only scan files with these extensions:
```
.dll, .onnx, .so, .pyd
```
This is useful for finding all ONNX model files or all native
libraries across your system.

**Exclude extensions** — skip files with these extensions:
```
.tmp, .log, .cache
```
This reduces noise from temporary and log files.

> Both filters are **case-insensitive**. `.DLL` and `.dll` are the same.

### 5.2 Path Segment Filters

**Include paths containing** — only scan files whose full path
contains one of these segments:
```
.nuget, onnx, models
```
This finds all files in paths that contain `.nuget` (NuGet cache),
`onnx` (ONNX-related), or `models` (ML model directories).

**Exclude paths containing** — skip files whose path contains:
```
bin, obj, .git
```
This excludes build output and git metadata from the scan.

### 5.3 Depth Limit

**Max depth** — how many levels deep to scan:
- `0`: Only the root folder (no subdirectories)
- `1`: Root + immediate subdirectories
- `5`: Five levels deep
- `-1`: Unlimited (default) — scan the entire tree

> A depth of 3-5 is often a good balance for initial exploration
> without scanning every single file on the drive.

### 5.4 Size Filters

**Min size** — only include files at or above this size (in bytes):
```
1048576   (1 MB)
1073741824  (1 GB)
```

**Max size** — only include files at or below this size:
```
1073741824  (1 GB)
```

> Setting "Min size" to 1 MB will quickly filter out thousands
> of tiny files and show you the space hogs.

### 5.5 Hidden and System Files

- **Skip hidden** (checked by default): Excludes files and directories
  with the Windows "Hidden" attribute (e.g. `C:\$Recycle.Bin`).
- **Skip system** (unchecked by default): Excludes files and directories
  with the Windows "System" attribute. Enable this if you want to
  avoid system-critical files.

### 5.6 Combining Filters

All filters are applied **simultaneously** (logical AND). A file is
included only if it passes **all** active filters:

```
A file is included if:
  (no include extensions OR extension is in include list)
  AND (no exclude extensions OR extension is not in exclude list)
  AND (no include paths OR path contains an include segment)
  AND (no exclude paths OR path does not contain an exclude segment)
  AND (size >= min size)
  AND (size <= max size)
  AND (not hidden OR skip hidden is unchecked)
  AND (not system OR skip system is unchecked)
```

## 6. Understanding the Results

### 6.1 Table Columns

| Column | What it shows |
|---|---|
| **Full Path** | The complete file system path (e.g. `C:\Users\me\.nuget\packages\...`) |
| **Name** | The file or folder name (e.g. `onnxruntime.dll`) |
| **Extension** | The file extension (e.g. `.dll`), empty for directories |
| **Size** | Human-readable size (e.g. `5.23 GB`) |
| **Depth** | How many levels below the scan root (0 = root, 1 = immediate child, etc.) |
| **Type** | `DIR` for directories, `FILE` for files |
| **Last Write** | When the file was last modified |
| **Duplicate** | `YES` if the file is part of a duplicate group |
| **Hash** | The SHA256 hash of the file (only shown if duplicate detection was enabled) |

### 6.2 Sorting

Click any column header to sort the table:

1. **First click:** Sort **ascending** (A→Z, small→large)
2. **Second click:** Sort **descending** (Z→A, large→small)
3. **Third click:** Return to **default order** (size descending)

The sort is applied in-memory to the full result list and the table
refreshes immediately.

> **Tip:** To find the largest files, click the **Size** column header
> once (ascending) or twice (descending). The largest files will be
> at the top.

### 6.3 Selecting Items for Cleanup

- **Select one item:** Click on a row.
- **Select multiple items:** Hold **Ctrl** and click additional rows.
- **Select a range:** Click the first row, hold **Shift**, click the last row.
- **Select all:** Press **Ctrl+A**.

The selected rows are highlighted in blue. The cleanup buttons
(Recycle Bin, Hard Delete) operate on the selected rows.

> If no rows are selected, the cleanup buttons will select **all
> visible** rows (after applying the current search/size filters).
> Be careful with this — it could mean thousands of files.

## 7. Searching and Filtering Results

### 7.1 Search Box

Type in the search box to filter results by **file name** or **full path**:

```
Search: dll       → shows all .dll files
Search: .nuget    → shows all files in the .nuget path
Search: onnx      → shows all files with "onnx" in the name or path
```

The search is **case-insensitive** and updates the table as you type.

### 7.2 Minimum Size Filter

Set the "Min size" NumericUpDown to filter out files below a threshold:

```
Min size: 1048576   → hides all files smaller than 1 MB
Min size: 1073741824 → shows only files 1 GB or larger
```

> **Common values:**
> | Value | Meaning |
> |---|---|
> | `0` | Show all (no filter) |
> | `1048576` | 1 MB and larger |
> | `10485760` | 10 MB and larger |
> | `1073741824` | 1 GB and larger |

## 8. System Information Panel

The System Info panel (right side) shows the sizes of well-known
space-hungry locations:

| Category | What it is |
|---|---|
| **Windows Temp** | `C:\Windows\Temp` — system-wide temporary files |
| **User Temp** | `%TEMP%` (usually `C:\Users\<you>\AppData\Local\Temp`) — your user's temp files |
| **Prefetch** | `C:\Windows\Prefetch` — Windows caches app launch data here. Safe to delete, but Windows will rebuild it. |
| **Recycle Bin** | Files you've deleted that are waiting to be permanently removed. Emptying the Recycle Bin reclaims this space. |
| **WinSxS** | `C:\Windows\WinSxS` — the Windows component store. Do NOT delete this manually. Use `DISM /Online /Cleanup-Image /StartComponentCleanup` instead. |
| **Service Profiles** | `C:\Windows\ServiceProfiles` — profiles for Windows services. |
| **ProgramData** | `C:\ProgramData` — system-wide application data. Contains caches and config for installed apps. |

> **Important:** The sizes shown here are approximate. Some of these
> locations require elevated (admin) permissions to scan fully.
> If a size shows 0 or is much lower than expected, it likely means
> the folder is access-protected. Re-run the app as Administrator
> (right-click the .exe → "Run as administrator") and click
> **Refresh System Info** again (see [Section 13](#13-uac-elevation)).

Click **Refresh System Info** to re-query all categories. This is
useful after you've cleaned up some temp files or emptied the
Recycle Bin.

## 9. Duplicate Detection

### 9.1 What It Does

Duplicate detection finds files that are **byte-identical** (same
content, regardless of name or location). It uses the SHA256 hash
of each file's content to identify duplicates.

### 9.2 How to Enable

1. In the Filter Panel, check the **"Detect duplicates (SHA256)"** checkbox.
2. Optionally, set the **"Min size for duplicate detection"** to
   skip small files (default: 1 MB = 1048576 bytes).
3. Click **Scan**.

After the scan completes, the status will show "Detecting duplicates..."
and the detection will run. When done, the results table will show
a "Duplicate" column with "YES" for files that have at least one
identical copy elsewhere.

### 9.3 Understanding the Results

- **Duplicate groups:** Files with the same hash are grouped together.
  The "Hash" column shows the SHA256 hash shared by all files in a group.
- **Potential savings:** For each group of N identical files of size S,
  the potential savings is `S × (N - 1)` — you can delete N-1 copies
  and keep one.
- **Sorting:** Duplicate groups are sorted by potential savings
  (largest first), so the most impactful duplicates appear at the top.

### 9.4 Performance

- Only files with the **same size** are hashed (files with unique sizes
  cannot have duplicates). This is a fast pre-filter.
- Hashing is parallelized across 4 threads.
- For a 900 GB drive with millions of files, duplicate detection
  can take **5-30 minutes** depending on the number of candidate files.
- The "Min size" filter is your best friend here: setting it to
  10 MB or 50 MB dramatically reduces the number of files to hash.

### 9.5 Common Duplicate Scenarios

- **ONNX model files** copied into multiple repositories (your exact scenario!)
- **NuGet package extracts** — the same package in multiple project caches
- **Build artifacts** — the same DLL compiled in multiple `bin/` folders
- **OS installer files** — Windows Update caches
- **Media files** — photos/videos copied to multiple locations

## 10. Cleaning Up: Recycle Bin vs. Hard Delete

### 10.1 Send to Recycle Bin (Recommended for First Pass)

**Button:** "Send to Recycle Bin"

- **What it does:** Moves the selected files/directories to the
  Windows Recycle Bin.
- **Reversible?** **Yes.** You can restore the files from the
  Recycle Bin if you change your mind.
- **Confirmation:** A dialog shows the count of items and total size
  before proceeding.
- **Best for:** First pass cleanup. Delete what you're confident
  about, put it in the Recycle Bin, and live with it for a few days.
  If you don't miss it, empty the Recycle Bin.

### 10.2 Hard Delete (Use with Caution)

**Button:** "Hard Delete (final)"

- **What it does:** Permanently deletes the selected files/directories
  from the file system.
- **Reversible?** **No.** The files are gone. Not to the Recycle Bin,
  not to any backup.
- **Confirmation:** Two-stage confirmation:
  1. First dialog: "PERMANENTLY DELETE N items? Total size: X.
     This action CANNOT be undone."
  2. If total size > 100 MB: second dialog "Are you absolutely sure?"
- **Best for:** Files you are 100% sure about — temp files, build
  artifacts, duplicate copies you've already verified.

### 10.3 Workflow Recommendation

```
1. Scan the drive.
2. Sort by size (largest first).
3. Review the top 50-100 entries.
4. For each item:
   a. Is it a temp file or build artifact? → Recycle Bin
   b. Is it a duplicate you've verified? → Recycle Bin (first)
   c. Is it a large file you no longer need? → Recycle Bin
   d. Are you 100% sure it's safe to delete? → Hard Delete
5. After a few days, empty the Recycle Bin for the items
   you didn't need.
```

### 10.4 What Happens After Cleanup

After a cleanup operation completes:
- The status label shows the result: "Recycle Bin: 42 succeeded, 3 failed.
  Freed: 5.23 GB"
- If any items failed, up to 5 error messages are shown.
- The results table is refreshed (deleted items are removed from the list).

## 11. Exporting Reports

### 11.1 When to Export

- **Before cleanup:** Export the scan results so you have a record
  of what was on your drive before you deleted anything.
- **For analysis:** Open the CSV in Excel to do pivot tables, or
  open the HTML in a browser for a nice formatted view.
- **For sharing:** Send the report to a colleague or for documentation.

### 11.2 How to Export

1. Click **Export Report...** in the Results Panel.
2. In the Save dialog, choose the location and filename.
3. Select the format from the file type dropdown:
   - **CSV:** Best for Excel analysis. One row per file/directory.
   - **JSON:** Best for programmatic processing. Pretty-printed.
   - **HTML:** Best for browser viewing. Styled table with summary.
   - **TXT:** Best for quick reference. One line per entry.
4. Click **Save**.

### 11.3 Report Contents

All formats include:
- A summary section (total files, total directories, total size, timestamp)
- One entry per file/directory with: path, name, extension, size,
  depth, type, last write time

The HTML format is the most visually appealing — it includes a
styled table with alternating row colors and a summary header.

## 12. Saving Filter Presets

### 12.1 What Presets Are

A **preset** is a saved set of filter options (extensions, paths,
depth, size limits, duplicate settings) with a name. Presets let
you quickly re-apply a filter configuration you use often.

### 12.2 Example Presets

| Preset Name | What it does |
|---|---|
| `ONNX DLLs` | Include: `.dll, .onnx, .so` / Exclude: `bin, obj` |
| `Large files only` | Min size: 1 GB / Max depth: -1 |
| `NuGet cache` | Include paths: `.nuget` / Min size: 10 MB |
| `Everything` | No filters — scan the whole drive |

### 12.3 How to Save a Preset

1. Configure the filters in the Filter Panel.
2. Click **Save Preset**.
3. Enter a name for the preset (e.g. "ONNX DLLs").
4. The preset is saved to `%AppData%\Win11DeTrasher\filters.json`.

### 12.4 How to Use a Preset

1. Select the preset name from the Preset ComboBox.
2. All filter fields are automatically populated.
3. Click **Scan** to start with those filters.

### 12.5 How to Delete a Preset

1. Select the preset from the Preset ComboBox.
2. Click **Delete Preset**.
3. The preset is removed from the JSON file.

> Presets are stored in plain JSON — you can edit `filters.json`
> directly with a text editor if you want to make bulk changes
> or share presets with a colleague.

## 13. UAC Elevation

### 13.1 What UAC Is

UAC (User Account Control) is Windows' security feature that
prompts for administrator credentials when an application needs
elevated (admin) rights.

### 13.2 How Win11DeTrasher Handles UAC

The application starts **without** elevation by default. When you
select a path that requires admin access (e.g. `C:\Windows`,
`C:\Program Files`) and click **Scan**, a dialog appears:

> **Elevation Required**
> This path may require elevated (UAC) permissions to scan.
> Would you like to re-run Win11DeTrasher as Administrator?
> Note: The current scan results will be lost.
> [Yes] [No]

- **Yes:** A new instance of the app starts with admin rights.
  The UAC prompt appears (the standard Windows "Do you want to
  allow this app to make changes?" dialog). The current window
  closes.
- **No:** The scan proceeds without elevation. Some files in
  protected directories will be inaccessible (shown as errors
  in the progress panel).

### 13.3 When Elevation Is Needed

| Path | Elevation needed? | Why |
|---|---|---|
| `C:\Windows` | Yes | System-protected |
| `C:\Program Files` | Yes | System-protected |
| `C:\Program Files (x86)` | Yes | System-protected |
| `C:\Users\<you>` | No | Your own files |
| `C:\Users\Public` | No | Public folder |
| `D:\`, `E:\`, etc. | Usually no | Depends on permissions |

> If you want to scan the entire C: drive including Windows,
> re-run the app as Administrator from the start (right-click
> the .exe → "Run as administrator").

## 14. Tips for C: Drive Cleanup

Here are the most common space hogs on a 900+ GB C: drive,
in order of typical impact:

### 14.1 Windows Temp and User Temp

- **Where:** `C:\Windows\Temp` and `C:\Users\<you>\AppData\Local\Temp`
- **Size:** Typically 1-10 GB
- **Safe to delete:** Yes — most files in temp folders are
  temporary by definition. Some may be in use by running apps.
- **How:** Use the System Info panel to check the size, then
  scan the temp folder and send files to Recycle Bin.

### 14.2 Recycle Bin

- **Where:** `C:\$Recycle.Bin` (hidden)
- **Size:** Varies — can be several GB
- **Safe to delete:** Yes — these are files you already deleted.
- **How:** Right-click the Recycle Bin icon on the desktop
  → "Empty Recycle Bin". Or use the System Info panel to
  check the size.

### 14.3 NuGet Cache

- **Where:** `C:\Users\<you>\.nuget\packages`
- **Size:** Can be 10-50+ GB for heavy .NET development
- **Safe to delete:** Yes — NuGet will re-download packages
  when needed. This is the single biggest space hog for
  .NET developers.
- **How:** Scan with "Include paths containing: `.nuget`"
  and "Min size: 1 MB". Review the results and send to
  Recycle Bin. Or simply delete the `.nuget\packages` folder
  (NuGet will rebuild it).

### 14.4 ONNX / ML Model Files

- **Where:** `C:\Users\<you>\source\repos\<various>\`
- **Size:** 1-20 GB per model file, and they get copied
  into every repository that uses them
- **Safe to delete:** **Carefully.** These are model weights
  or runtime binaries. If you delete them, you need to
  re-download or re-clone the repositories.
- **How:** Scan with "Include extensions: `.onnx, .dll, .so, .pyd`"
  and "Min size: 100 MB". Use duplicate detection to find
  identical copies across repositories. Keep one copy,
  Recycle Bin the rest.

### 14.5 Build Artifacts

- **Where:** `C:\Users\<you>\source\repos\<repo>\bin\`, `obj\`,
  `build\`, `out\`
- **Size:** 1-20 GB per repository
- **Safe to delete:** Yes — these are generated by the build
  process and can be regenerated.
- **How:** Scan with "Exclude paths containing: `bin, obj`"
  (to see what you're NOT scanning) or "Include paths
  containing: `bin, obj`" (to find all build artifacts).

### 14.6 Windows Update Cache

- **Where:** `C:\Windows\SoftwareDistribution\Download`
- **Size:** 1-20 GB
- **Safe to delete:** Yes — Windows will re-download updates
  as needed. Stop the Windows Update service first.
- **How:** Use "Disk Cleanup" (cleanmgr) → "Windows Update
  Cleanup" for a safe removal. Or scan this folder and
  Recycle Bin the files.

### 14.7 Delivery Optimization

- **Where:** `C:\Windows\SoftwareDistribution` and
  `C:\Windows\System32\catroot2`
- **Size:** 1-10 GB
- **Safe to delete:** Yes — cached Windows Update files.
- **How:** Use "Disk Cleanup" or "Settings → Storage →
  Temporary files".

### 14.8 Visual Studio and IDE Caches

- **Where:** `C:\Users\<you>\AppData\Local\Microsoft\VisualStudio`,
  `C:\Users\<you>\AppData\Local\JetBrains`
- **Size:** 1-20 GB
- **Safe to delete:** Mostly — IDE caches can be regenerated.
  Be cautious with "Projects" subfolders.
- **How:** Scan with "Include paths containing: `AppData\Local\Microsoft`"
  and "Min size: 100 MB".

### 14.9 Large Media Files

- **Where:** `C:\Users\<you>\Downloads`, `Pictures`, `Videos`
- **Size:** Varies
- **Safe to delete:** Your call — check before deleting.
- **How:** Scan with "Min size: 1 GB" and "Sort by size"
  to find your largest media files.

### 14.10 Docker / Hyper-V Images

- **Where:** `C:\ProgramData\Docker`,
  `C:\Users\<you>\AppData\Local\Docker`
- **Size:** 10-100+ GB
- **Safe to delete:** **Carefully** — if you use Docker
  images, deleting them means re-pulling.
- **How:** Use `docker system prune` for Docker. For
  Hyper-V, use PowerShell:
  ```powershell
  Get-VM | Get-VHD | Remove-VHD
  ```
  To find which VHDs are the largest, run:
  ```powershell
  Get-VM | Select-Object Name, @{N='SizeGB';E={[math]::Round($_.HardDrives[0].Size/1GB,2)}}
  ```

## 15. Troubleshooting

### 15.1 "Scan error: A scan is already in progress"

You tried to start a second scan while the first one was still
running. Wait for the first scan to complete or cancel it first.

### 15.2 "Some files could not be accessed (Errors: N)"

This is normal when scanning system directories without admin
rights. The files in `C:\Windows`, `C:\Program Files`, etc.
require elevation. Re-run the app as Administrator to scan
these locations fully.

### 15.3 "The UAC prompt was cancelled"

You pressed "Cancel" on the UAC dialog. The app continues
without elevation. You can retry by selecting the protected
path again and choosing "Yes" on the elevation prompt.

### 15.4 "Failed to export the report"

Check that the file path is valid and that you have write
permissions to the destination folder. Also check that the
file isn't already open in another program (e.g. Excel).

### 15.5 "The system feels slow during the scan"

Reduce the worker count in the WorkerControl. The scan will
slow down, but the system will be more responsive. You can
increase the count again later when the heavy I/O phase
has passed.

### 15.6 "Duplicate detection takes forever"

This is expected for large drives with many files. Consider:
- Increasing the "Min size for duplicate detection" (e.g. to
  50 MB or 100 MB) to reduce the number of files to hash.
- Using a more specific filter (e.g. only scan `source\repos`
  instead of the whole C: drive).
- Letting it run in the background — the UI remains responsive
  because the detection runs on background threads.

### 15.7 "I deleted a file I needed"

If you used "Send to Recycle Bin": open the Recycle Bin
and restore the file. If you used "Hard Delete": the file
is gone. Check for backups. For future reference, use
"Send to Recycle Bin" for anything you're not 100% sure about.

## 16. Glossary

| Term | Definition |
|---|---|
| **ETA** | Estimated Time of Arrival — how long until the scan finishes. |
| **SHA256** | A cryptographic hash function that produces a 256-bit (32-byte) fingerprint of a file's content. Two files with the same SHA256 hash are almost certainly identical. |
| **UAC** | User Account Control — Windows' security prompt for admin rights. |
| **Recycle Bin** | A hidden folder (`C:\$Recycle.Bin`) where deleted files are stored before permanent removal. |
| **WinSxS** | Windows Side-by-Side — the component store that holds all Windows system components. Do not delete manually. |
| **NuGet** | The .NET package manager. Its cache (`~/.nuget/packages`) can grow very large. |
| **ONNX** | Open Neural Network Exchange — a format for machine learning models. Model files can be gigabytes in size. |
| **Worker thread** | A background thread that processes files from the scan queue. The number of workers controls parallelism. |
| **Depth** | How many levels below the scan root a file or directory is. The root is depth 0, its immediate children are depth 1, etc. |
| **Preset** | A saved set of filter options with a name, stored in a JSON file. |
| **Hard Delete** | Permanent file removal. Files are gone and cannot be recovered. |
| **Recycle Bin (operation)** | Moving files to `C:\$Recycle.Bin` via the Windows Shell API. Reversible. |

---

## 17. Footnotes — Technical Details

<sup>1</sup> **ETA reliability:** The ETA is computed as `(remaining bytes) / (average throughput)`. It becomes reliable only after at least 1 second of elapsed time and some progress. If the total byte count is still growing (files still being discovered), the ETA will fluctuate — this is expected.

<sup>2</sup> **Worker count adjustment:** Increasing the worker count during a scan starts new threads immediately. Decreasing it does not forcibly terminate existing threads — they finish their current item and exit when the queue is empty. This avoids data loss. In practice, excess threads exit within a few seconds.

<sup>3</sup> **In-memory result storage:** All scan results are kept in memory. For a 900 GB drive with 5 million files, this uses roughly 2–5 GB of RAM. If your system has less than 16 GB of RAM, use the min-size filter to reduce the number of results.

<sup>4</sup> **Duplicate detection performance:** Only files with the same size are hashed (a fast pre-filter eliminates most files). Hashing uses 4 threads with 80 KB read buffers. For typical file sets, only 5–20% of files have duplicate sizes.

<sup>5</sup> **Preset file location:** `%AppData%\Win11DeTrasher\filters.json` (typically `C:\Users\<you>\AppData\Roaming\Win11DeTrasher\filters.json`). The file is plain JSON — you can edit it directly with a text editor.

<sup>6</sup> **Recycle Bin P/Invoke:** The app uses `SHFileOperation` with `FO_DELETE` + `FOF_ALLOWUNDO` to send items to the Recycle Bin. The `FOF_NOCONFIRMATION` and `FOF_SILENT` flags suppress the Windows confirmation dialog (the app shows its own).

<sup>7</sup> **Elevation check:** `ElevationHelper.IsElevated()` queries the current process token via `OpenProcessToken` + `GetTokenInformation` (class 12 = `TokenIsElevated`). No WinRT API is used.

<sup>8</sup> **Progress throttling:** Progress events are throttled to at most 5 per second (200 ms minimum interval) to avoid flooding the UI thread. Completion and cancellation events are always raised immediately.

<sup>9</sup> **CSV escaping:** Fields containing commas, quotes, or newlines are wrapped in double quotes, with internal quotes doubled (standard CSV escaping). The file is written in UTF-8.

<sup>10</sup> **Sort implementation:** The `SortableDataGridView` sorts the bound `List<ScanResult>` in place using `List.Sort` with a reflection-based comparator (reads the column's `DataPropertyName` property). The DataSource is then reset to trigger a full grid refresh.

---

*End of User Manual*
