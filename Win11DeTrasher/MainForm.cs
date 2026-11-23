using System.ComponentModel;
using System.Windows.Forms;
using Win11DeTrasher.Controls;
using Win11DeTrasher.Models;
using Win11DeTrasher.Services;
using Win11DeTrasher.UI;
using Win11DeTrasher.Utils;

namespace Win11DeTrasher;

/// <summary>
/// The main form of the Win11DeTrasher application.
/// Provides drive selection, scan filter configuration, real-time progress
/// display, a sortable results table with search and size filtering,
/// system info panel, duplicate detection, and cleanup actions
/// (Recycle Bin and Hard Delete).
/// </summary>
public partial class MainForm : Form
{
    private readonly IConfigService _configService;
    private readonly IDriveDiscoveryService _driveService;
    private readonly IProgressTracker _progressTracker;
    private readonly IFileScanner _scanner;
    private readonly IDuplicationDetector _duplicationDetector;
    private readonly ISystemInfoService _systemInfoService;
    private readonly IReportExporter _reportExporter;
    private readonly ICleanupService _cleanupService;

    private CancellationTokenSource? _scanCts;
    private List<ScanResult> _allResults = [];
    private List<DuplicateGroup> _duplicateGroups = [];
    private System.Windows.Forms.BindingSource _resultsBindingSource = new();

    // Progress UI state
    private bool _isScanRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm(
        IConfigService configService,
        IDriveDiscoveryService driveService,
        IProgressTracker progressTracker,
        IFileScanner scanner,
        IDuplicationDetector duplicationDetector,
        ISystemInfoService systemInfoService,
        IReportExporter reportExporter,
        ICleanupService cleanupService)
    {
        this._configService = configService;
        this._driveService = driveService;
        this._progressTracker = progressTracker;
        this._scanner = scanner;
        this._duplicationDetector = duplicationDetector;
        this._systemInfoService = systemInfoService;
        this._reportExporter = reportExporter;
        this._cleanupService = cleanupService;

        this.InitializeComponent();

        this.Load += this.OnFormLoad;
        this.FormClosing += this.OnFormClosing;
        this.Disposed += (_, _) => this.OnFormDisposed();

        // Subscribe to progress updates
        this._progressTracker.ProgressUpdated += this.OnProgressUpdated;
    }

    private async void OnFormLoad(object? sender, EventArgs e)
    {
        await this.PopulateDrivesAsync();
        await this.PopulateSystemInfoAsync();
        this.PopulatePresets();
    }

    private async Task PopulateDrivesAsync()
    {
        var drives = await this._driveService.GetAvailableDrivesAsync();

        this._driveComboBox.Items.Clear();
        foreach (var drive in drives)
        {
            var display = $"{drive.Letter}:\\  ({drive.DriveType} — {SizeFormatter.Format(drive.TotalBytes)} total, {SizeFormatter.Format(drive.FreeBytes)} free)";
            this._driveComboBox.Items.Add(new DriveComboBoxItem(drive, display));
        }

        // Add "Custom path..." option
        this._driveComboBox.Items.Add(new DriveComboBoxItem(null, "...  (browse to a specific folder)"));

        // Select C: by default if available
        var cItem = this._driveComboBox.Items.Cast<DriveComboBoxItem>().FirstOrDefault(d => d.Drive?.Letter.StartsWith("C", StringComparison.OrdinalIgnoreCase) == true);
        this._driveComboBox.SelectedItem = cItem ?? (this._driveComboBox.Items.Count > 0 ? this._driveComboBox.Items[0] : null);
    }

    private void PopulatePresets()
    {
        this._presetComboBox.Items.Clear();
        var presets = this._configService.LoadPresets();

        foreach (var preset in presets)
        {
            this._presetComboBox.Items.Add(preset.Name);
        }

        if (this._presetComboBox.Items.Count > 0)
        {
            this._presetComboBox.SelectedIndex = 0;
        }
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        // Cancel any running scan
        if (this._isScanRunning)
        {
            this._scanCts?.Cancel();
        }

        this._scanner.Dispose();
        this._progressTracker.Dispose();
    }

    // --- Progress handling ---

    private void OnProgressUpdated(ScanProgress progress)
    {
        // Must be marshaled to the UI thread
        this.Invoke(() =>
        {
            this._progressBar.Value = (int)Math.Clamp(progress.ProgressPercent, 0, 100);
            this._elapsedLabel.Text = $"Elapsed: {this.FormatTimeSpan(progress.ElapsedSeconds)}";
            this._etaLabel.Text = progress.EstimatedRemainingSeconds > 0
                ? $"ETA: {this.FormatTimeSpan(progress.EstimatedRemainingSeconds)}"
                : "ETA: calculating...";
            this._fileCountLabel.Text = $"Files: {progress.ProcessedFiles:N0} / {progress.TotalFiles:N0}";
            this._errorCountLabel.Text = $"Errors: {progress.ErrorCount:N0}";

            if (progress.IsCompleted)
            {
                this._statusLabel.Text = "Scan complete.";
                this._scanButton.Enabled = true;
                this._cancelButton.Enabled = false;
                this._workerControl.IsEditable = true;
                this._isScanRunning = false;

                this.PopulateResults();
            }
            else if (progress.IsCancelled)
            {
                this._statusLabel.Text = "Scan cancelled.";
                this._scanButton.Enabled = true;
                this._cancelButton.Enabled = false;
                this._workerControl.IsEditable = true;
                this._isScanRunning = false;

                this.PopulateResults();
            }
        });
    }

    private string FormatTimeSpan(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        if (ts.TotalHours >= 1)
        {
            return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
        }

        if (ts.TotalMinutes >= 1)
        {
            return $"{ts.Minutes}m {ts.Seconds}s";
        }

        return $"{ts.Seconds:F1}s";
    }

    // --- Scan start/stop ---

    private async void OnScanButtonClick(object? sender, EventArgs e)
    {
        var rootPath = this.GetSelectedPath();
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            MessageBox.Show(this, "Please select a valid path to scan.", "No Path",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!Directory.Exists(rootPath))
        {
            MessageBox.Show(this, $"Path does not exist: {rootPath}", "Invalid Path",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Check if elevation is needed
        if (!ElevationHelper.IsElevated() && ElevationHelper.RequiresElevation(rootPath))
        {
            var result = MessageBox.Show(this,
                "This path may require elevated (UAC) permissions to scan.\n\n" +
                "Would you like to re-run Win11DeTrasher as Administrator?\n\n" +
                "Note: The current scan results will be lost.",
                "Elevation Required",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (ElevationHelper.LaunchElevated())
                {
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show(this, "The UAC prompt was cancelled.\nYou can still scan this path, but some files may be inaccessible.",
                        "Elevation Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                // User chose to continue without elevation
            }
        }

        // Build filter options from current UI state
        var filterOptions = this.BuildFilterOptionsFromUi();

        // Start the scan
        this._isScanRunning = true;
        this._scanCts = new CancellationTokenSource();

        this._scanButton.Enabled = false;
        this._cancelButton.Enabled = true;
        this._workerControl.IsEditable = true;
        this._statusLabel.Text = "Scanning...";
        this._progressBar.Value = 0;
        this._resultsBindingSource.DataSource = null;
        this._resultsDataGridView.Rows.Clear();

        try
        {
            var results = await this._scanner.ScanAsync(rootPath, filterOptions, this._scanCts.Token);
            this._allResults = results.ToList();

            // Run duplicate detection if enabled
            if (filterOptions.DetectDuplicates)
            {
                this._statusLabel.Text = "Detecting duplicates...";
                this._duplicateGroups = (await this._duplicationDetector.DetectDuplicatesAsync(
                    this._allResults,
                    filterOptions.DuplicateMinSizeBytes,
                    this._scanCts.Token)).ToList();

                // Assign duplicate group ids to scan results
                this.AssignDuplicateGroupIds();
            }
            else
            {
                this._duplicateGroups = [];
            }

            this.PopulateResults();
        }
        catch (OperationCanceledException)
        {
            this._statusLabel.Text = "Scan cancelled.";
        }
        catch (Exception ex)
        {
            this._statusLabel.Text = $"Scan error: {ex.Message}";
            MessageBox.Show(this, ex.Message, "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            this._isScanRunning = false;
            this._scanButton.Enabled = true;
            this._cancelButton.Enabled = false;
        }
    }

    private void OnCancelButtonClick(object? sender, EventArgs e)
    {
        this._scanCts?.Cancel();
    }

    private string GetSelectedPath()
    {
        if (this._driveComboBox.SelectedItem is not DriveComboBoxItem item)
        {
            return string.Empty;
        }

        if (item.Drive == null)
        {
            // Custom path — open folder browser
            using var folderBrowser = new FolderBrowserDialog
            {
                Description = "Select a directory to scan",
                UseDescriptionForTitle = true
            };

            return folderBrowser.ShowDialog() == DialogResult.OK ? folderBrowser.SelectedPath : string.Empty;
        }

        return item.Drive.Letter + "\\";
    }

    private ScanFilterOptions BuildFilterOptionsFromUi()
    {
        var options = new ScanFilterOptions();

        // Apply preset if selected
        if (this._presetComboBox.SelectedItem is string presetName)
        {
            var presets = this._configService.LoadPresets();
            var preset = presets.FirstOrDefault(p => p.Name == presetName);
            if (preset != null)
            {
                options = preset.Options;
            }
        }

        // Override with UI values
        if (!string.IsNullOrEmpty(this._includeExtensionsBox?.Text))
        {
            options.IncludeExtensions = this.ParseCommaList(this._includeExtensionsBox.Text);
        }

        if (!string.IsNullOrEmpty(this._excludeExtensionsBox?.Text))
        {
            options.ExcludeExtensions = this.ParseCommaList(this._excludeExtensionsBox.Text);
        }

        if (!string.IsNullOrEmpty(this._includePathsBox?.Text))
        {
            options.IncludePathSegments = this.ParseCommaList(this._includePathsBox.Text);
        }

        if (!string.IsNullOrEmpty(this._excludePathsBox?.Text))
        {
            options.ExcludePathSegments = this.ParseCommaList(this._excludePathsBox.Text);
        }

        if (this._maxDepthBox?.Value != null)
        {
            options.MaxDepth = (int)this._maxDepthBox.Value;
        }

        if (this._minSizeBox?.Value != null)
        {
            options.MinFileSizeBytes = (long)this._minSizeBox.Value;
        }

        if (this._maxSizeBox?.Value != null)
        {
            options.MaxFileSizeBytes = (long)this._maxSizeBox.Value;
        }

        options.SkipHidden = this._skipHiddenCheck?.Checked ?? true;
        options.SkipSystem = this._skipSystemCheck?.Checked ?? false;
        options.DetectDuplicates = this._detectDuplicatesCheck?.Checked ?? false;

        if (this._duplicateMinSizeBox?.Value != null)
        {
            options.DuplicateMinSizeBytes = (long)this._duplicateMinSizeBox.Value;
        }

        return options;
    }

    private List<string> ParseCommaList(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        return text
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => s.Length > 0)
            .ToList();
    }

    private void AssignDuplicateGroupIds()
    {
        foreach (var group in this._duplicateGroups)
        {
            foreach (var file in group.Files)
            {
                var result = this._allResults.FirstOrDefault(r => r.FullPath == file);
                if (result != null)
                {
                    result.DuplicateGroupId = group.GroupId;
                    result.Hash = group.Hash;
                }
            }
        }
    }

    // --- Results display ---

    private void PopulateResults()
    {
        // Apply search and size filters
        var filtered = this._allResults;

        var searchQuery = this._searchBox?.Text?.Trim();
        if (!string.IsNullOrEmpty(searchQuery))
        {
            filtered = filtered
                .Where(r => r.FullPath.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                          r.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var minSizeFilter = (long)(this._minSizeFilterBox?.Value ?? 0);
        if (minSizeFilter > 0)
        {
            filtered = filtered
                .Where(r => !r.IsDirectory || r.SizeBytes >= minSizeFilter)
                .ToList();
        }

        // Sort by size descending by default
        filtered = filtered
            .OrderByDescending(r => r.SizeBytes)
            .ToList();

        this._resultsBindingSource.DataSource = filtered;
    }

    private void OnSearchBoxTextChanged(object? sender, EventArgs e)
    {
        if (this._allResults.Count > 0)
        {
            this.PopulateResults();
        }
    }

    private void OnMinSizeFilterChanged(object? sender, EventArgs e)
    {
        if (this._allResults.Count > 0)
        {
            this.PopulateResults();
        }
    }

    // --- System info ---

    private async void OnRefreshSystemInfoClick(object? sender, EventArgs e)
    {
        this._refreshSystemInfoButton.Enabled = false;
        this._statusLabel.Text = "Gathering system info...";

        try
        {
            await this.PopulateSystemInfoAsync();
        }
        finally
        {
            this._refreshSystemInfoButton.Enabled = true;
        }
    }

    private async Task PopulateSystemInfoAsync()
    {
        var entries = await this._systemInfoService.GetSystemInfoAsync(CancellationToken.None);

        this._systemInfoGrid.Rows.Clear();
        foreach (var entry in entries)
        {
            this._systemInfoGrid.Rows.Add(
                entry.Category,
                entry.Path,
                SizeFormatter.Format(entry.SizeBytes),
                entry.Description);
        }

        // Add total
        var totalSize = entries.Sum(e => e.SizeBytes);
        this._systemInfoGrid.Rows.Add("Total", "", SizeFormatter.Format(totalSize), "Sum of all categories above");
    }

    // --- Export ---

    private async void OnExportButtonClick(object? sender, EventArgs e)
    {
        if (this._allResults.Count == 0)
        {
            MessageBox.Show(this, "No scan results to export. Please run a scan first.", "No Data",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var saveDialog = new SaveFileDialog
        {
            Filter = "All supported formats (*.csv;*.json;*.html;*.txt)|*.csv;*.json;*.html;*.txt|CSV (*.csv)|*.csv|JSON (*.json)|*.json|HTML (*.html)|*.html|Text (*.txt)|*.txt",
            Title = "Export Scan Report",
            FileName = $"Win11DeTrasher_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
        };

        if (saveDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var extension = Path.GetExtension(saveDialog.FileName).ToLowerInvariant();
        bool success = extension switch
        {
            ".csv" => this._reportExporter.ExportCsv(this._allResults, saveDialog.FileName),
            ".json" => this._reportExporter.ExportJson(this._allResults, saveDialog.FileName),
            ".html" => this._reportExporter.ExportHtml(this._allResults, saveDialog.FileName),
            ".txt" => this._reportExporter.ExportTxt(this._allResults, saveDialog.FileName),
            _ => false
        };

        if (success)
        {
            MessageBox.Show(this, $"Report exported to:\n{saveDialog.FileName}", "Export Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show(this, "Failed to export the report. Please check the file path and try again.", "Export Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // --- Cleanup ---

    private void OnRecycleBinButtonClick(object? sender, EventArgs e)
    {
        var paths = this.GetSelectedResultPaths();
        if (paths.Count == 0)
        {
            MessageBox.Show(this, "No items selected. Please select one or more items in the results table.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var totalSize = this._cleanupService.GetTotalSize(paths);

        var result = MessageBox.Show(this,
            $"Send {paths.Count:N0} item(s) to the Recycle Bin?\n\n" +
            $"Total size: {SizeFormatter.Format(totalSize)}\n\n" +
            "Items can be restored from the Recycle Bin.",
            "Confirm Recycle Bin",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        this._statusLabel.Text = $"Moving {paths.Count:N0} item(s) to Recycle Bin...";

        Task.Run(async () =>
        {
            var batchResult = await this._cleanupService.SendToRecycleBinAsync(paths, CancellationToken.None);

            this.Invoke(() =>
            {
                this._statusLabel.Text = $"Recycle Bin: {batchResult.Succeeded:N0} succeeded, {batchResult.Failed:N0} failed. Freed: {SizeFormatter.Format(batchResult.TotalFreedBytes)}";

                if (batchResult.Failed > 0)
                {
                    var errors = string.Join("\n", batchResult.Results.Where(r => !r.Success).Take(5).Select(r => r.ErrorMessage));
                    MessageBox.Show(this, $"Some items could not be moved:\n\n{errors}", "Partial Failure",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Re-scan the affected area to update results
                this.PopulateResults();
            });
        });
    }

    private void OnHardDeleteButtonClick(object? sender, EventArgs e)
    {
        var paths = this.GetSelectedResultPaths();
        if (paths.Count == 0)
        {
            MessageBox.Show(this, "No items selected. Please select one or more items in the results table.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var totalSize = this._cleanupService.GetTotalSize(paths);

        var result = MessageBox.Show(this,
            $"PERMANENTLY DELETE {paths.Count:N0} item(s)?\n\n" +
            $"Total size: {SizeFormatter.Format(totalSize)}\n\n" +
            "This action CANNOT be undone. The items will be permanently removed from the file system.",
            "Confirm Hard Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
        {
            return;
        }

        // Second confirmation for large deletions
        if (totalSize > 100 * 1024 * 1024) // > 100 MB
        {
            var result2 = MessageBox.Show(this,
                "You are about to permanently delete more than 100 MB of data.\n\nAre you absolutely sure?",
                "Final Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result2 != DialogResult.Yes)
            {
                return;
            }
        }

        this._statusLabel.Text = $"Permanently deleting {paths.Count:N0} item(s)...";

        Task.Run(async () =>
        {
            var batchResult = await this._cleanupService.HardDeleteAsync(paths, CancellationToken.None);

            this.Invoke(() =>
            {
                this._statusLabel.Text = $"Hard delete: {batchResult.Succeeded:N0} succeeded, {batchResult.Failed:N0} failed. Freed: {SizeFormatter.Format(batchResult.TotalFreedBytes)}";

                if (batchResult.Failed > 0)
                {
                    var errors = string.Join("\n", batchResult.Results.Where(r => !r.Success).Take(5).Select(r => r.ErrorMessage));
                    MessageBox.Show(this, $"Some items could not be deleted:\n\n{errors}", "Partial Failure",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.PopulateResults();
            });
        });
    }

    private List<string> GetSelectedResultPaths()
    {
        var paths = new List<string>();

        foreach (DataGridViewRow row in this._resultsDataGridView.SelectedRows)
        {
            var path = row.Cells["FullPath"]?.Value?.ToString();
            if (!string.IsNullOrEmpty(path))
            {
                paths.Add(path);
            }
        }

        // If no rows are selected but there are visible rows, select all visible
        if (paths.Count == 0 && this._resultsBindingSource.Count > 0)
        {
            foreach (var item in this._resultsBindingSource.DataSource as System.Collections.IEnumerable ?? Enumerable.Empty<object>())
            {
                var prop = item.GetType().GetProperty("FullPath");
                var path = prop?.GetValue(item)?.ToString();
                if (!string.IsNullOrEmpty(path))
                {
                    paths.Add(path);
                }
            }
        }

        return paths;
    }

    // --- Worker control ---

    private void OnWorkerCountChanged(int count)
    {
        this._scanner.WorkerCount = count;
        this._statusLabel.Text = $"Workers set to {count}.";
    }

    private void OnFilterBoxChanged(object? sender, EventArgs e)
    {
        // When filter boxes change, re-apply filters to the current results
        if (this._allResults.Count > 0)
        {
            this.PopulateResults();
        }
    }

    // --- Preset management ---

    private void OnPresetComboBoxSelectedIndexChanged(object? sender, EventArgs e)
    {
        // When a preset is selected, populate the filter fields
        if (this._presetComboBox.SelectedItem is string presetName)
        {
            var presets = this._configService.LoadPresets();
            var preset = presets.FirstOrDefault(p => p.Name == presetName);

            if (preset != null)
            {
                var opts = preset.Options;

                this._includeExtensionsBox.Text = string.Join(", ", opts.IncludeExtensions);
                this._excludeExtensionsBox.Text = string.Join(", ", opts.ExcludeExtensions);
                this._includePathsBox.Text = string.Join(", ", opts.IncludePathSegments);
                this._excludePathsBox.Text = string.Join(", ", opts.ExcludePathSegments);
                this._maxDepthBox.Value = Math.Clamp(opts.MaxDepth, this._maxDepthBox.Minimum, this._maxDepthBox.Maximum);
                this._minSizeBox.Value = Math.Clamp(opts.MinFileSizeBytes, 0, this._minSizeBox.Maximum);
                this._maxSizeBox.Value = opts.MaxFileSizeBytes < long.MaxValue ? opts.MaxFileSizeBytes : this._maxSizeBox.Maximum;
                this._skipHiddenCheck.Checked = opts.SkipHidden;
                this._skipSystemCheck.Checked = opts.SkipSystem;
                this._detectDuplicatesCheck.Checked = opts.DetectDuplicates;
                this._duplicateMinSizeBox.Value = Math.Clamp(opts.DuplicateMinSizeBytes, 0, this._duplicateMinSizeBox.Maximum);
            }
        }
    }

    private void OnSavePresetButtonClick(object? sender, EventArgs e)
    {
        var options = this.BuildFilterOptionsFromUi();

        var nameInput = this.PromptForPresetName();
        if (string.IsNullOrWhiteSpace(nameInput))
        {
            return;
        }

        var presets = this._configService.LoadPresets().ToList();

        // Remove existing preset with same name
        presets.RemoveAll(p => p.Name == nameInput);
        presets.Add(new ScanFilterPreset(nameInput, options));

        if (this._configService.SavePresets(presets))
        {
            this.PopulatePresets();
            var newItem = this._presetComboBox.Items.Cast<object>().FirstOrDefault(i => i?.ToString() == nameInput);
            this._presetComboBox.SelectedItem = newItem;
            this._statusLabel.Text = $"Preset '{nameInput}' saved.";
        }
        else
        {
            MessageBox.Show(this, "Failed to save preset.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string PromptForPresetName()
    {
        using var inputForm = new Form
        {
            Text = "Save Filter Preset",
            Size = new System.Drawing.Size(400, 160),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            StartPosition = FormStartPosition.CenterParent
        };

        var label = new Label
        {
            Text = "Preset name:",
            Dock = DockStyle.Top,
            Height = 30
        };

        var textBox = new TextBox
        {
            Dock = DockStyle.Fill
        };

        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Bottom,
            Height = 40
        };

        var okButton = new Button { Text = "OK", Width = 70 };
        okButton.Click += (_, _) => inputForm.Close();

        var cancelButton = new Button { Text = "Cancel", Width = 70 };
        cancelButton.Click += (_, _) => inputForm.DialogResult = DialogResult.Cancel;

        buttonPanel.Controls.Add(okButton);
        buttonPanel.Controls.Add(cancelButton);

        inputForm.Controls.Add(label);
        inputForm.Controls.Add(textBox);
        inputForm.Controls.Add(buttonPanel);
        inputForm.AcceptButton = okButton;
        inputForm.CancelButton = cancelButton;

        return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text.Trim() : string.Empty;
    }

    // --- Nested types ---

    /// <summary>
    /// A wrapper for ComboBox items that pairs a drive entry with its display string.
    /// </summary>
    /// <param name="Drive">The drive entry, or null for the custom-path option.</param>
    /// <param name="Display">The text shown in the ComboBox.</param>
    private sealed record DriveComboBoxItem(DriveEntry? Drive, string Display)
    {
        public override string ToString() => this.Display;
    }

    private void OnDeletePresetButtonClick(object? sender, EventArgs e)
    {
        if (this._presetComboBox.SelectedItem?.ToString() is { } presetName)
        {
            var presets = this._configService.LoadPresets().ToList();
            presets.RemoveAll(p => p.Name == presetName);

            if (this._configService.SavePresets(presets))
            {
                this.PopulatePresets();
                this._statusLabel.Text = $"Preset '{presetName}' deleted.";
            }
        }
    }

    /// <summary>
    /// Called when the form is being disposed. Unsubscribes from
    /// the progress tracker and disposes the binding source.
    /// </summary>
    private void OnFormDisposed()
    {
        this._progressTracker.ProgressUpdated -= this.OnProgressUpdated;
        this._resultsBindingSource.Dispose();
    }
}
