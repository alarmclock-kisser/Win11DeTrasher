#nullable enable
namespace Win11DeTrasher;

partial class MainForm
{
    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this._mainTable = new System.Windows.Forms.TableLayoutPanel();
        this._topPanel = new System.Windows.Forms.TableLayoutPanel();
        this._driveLabel = new System.Windows.Forms.Label();
        this._driveComboBox = new System.Windows.Forms.ComboBox();
        this._scanButton = new System.Windows.Forms.Button();
        this._cancelButton = new System.Windows.Forms.Button();
        this._workerControl = new Controls.WorkerControl();
        this._presetLabel = new System.Windows.Forms.Label();
        this._presetComboBox = new System.Windows.Forms.ComboBox();
        this._savePresetButton = new System.Windows.Forms.Button();
        this._deletePresetButton = new System.Windows.Forms.Button();
        this._filterPanel = new System.Windows.Forms.SplitContainer();
        this._contentSplit = new System.Windows.Forms.SplitContainer();
        this._includeExtensionsBox = new System.Windows.Forms.TextBox();
        this._excludeExtensionsBox = new System.Windows.Forms.TextBox();
        this._includePathsBox = new System.Windows.Forms.TextBox();
        this._excludePathsBox = new System.Windows.Forms.TextBox();
        this._maxDepthBox = new System.Windows.Forms.NumericUpDown();
        this._minSizeBox = new System.Windows.Forms.NumericUpDown();
        this._maxSizeBox = new System.Windows.Forms.NumericUpDown();
        this._skipHiddenCheck = new System.Windows.Forms.CheckBox();
        this._skipSystemCheck = new System.Windows.Forms.CheckBox();
        this._detectDuplicatesCheck = new System.Windows.Forms.CheckBox();
        this._duplicateMinSizeBox = new System.Windows.Forms.NumericUpDown();
        this._systemInfoPanel = new System.Windows.Forms.TableLayoutPanel();
        this._refreshSystemInfoButton = new System.Windows.Forms.Button();
        this._systemInfoGrid = new System.Windows.Forms.DataGridView();
        this._progressPanel = new System.Windows.Forms.TableLayoutPanel();
        this._progressBar = new System.Windows.Forms.ProgressBar();
        this._elapsedLabel = new System.Windows.Forms.Label();
        this._etaLabel = new System.Windows.Forms.Label();
        this._fileCountLabel = new System.Windows.Forms.Label();
        this._errorCountLabel = new System.Windows.Forms.Label();
        this._statusLabel = new System.Windows.Forms.Label();
        this._resultsPanel = new System.Windows.Forms.TableLayoutPanel();
        this._resultsSearchPanel = new System.Windows.Forms.FlowLayoutPanel();
        this._searchLabel = new System.Windows.Forms.Label();
        this._searchBox = new System.Windows.Forms.TextBox();
        this._minSizeFilterLabel = new System.Windows.Forms.Label();
        this._minSizeFilterBox = new System.Windows.Forms.NumericUpDown();
        this._resultsDataGridView = new Controls.SortableDataGridView();
        this._resultsButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
        this._exportButton = new System.Windows.Forms.Button();
        this._recycleBinButton = new System.Windows.Forms.Button();
        this._hardDeleteButton = new System.Windows.Forms.Button();
        this._logPanel = new System.Windows.Forms.TableLayoutPanel();
        this._logHeader = new System.Windows.Forms.TableLayoutPanel();
        this._logTitle = new System.Windows.Forms.Label();
        this._copyLogButton = new System.Windows.Forms.Button();
        this._listBox_log = new System.Windows.Forms.ListBox();

        ((System.ComponentModel.ISupportInitialize)this._filterPanel).BeginInit();
        this._filterPanel.Panel1.SuspendLayout();
        this._filterPanel.Panel2.SuspendLayout();
        this._filterPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this._contentSplit).BeginInit();
        this._contentSplit.Panel1.SuspendLayout();
        this._contentSplit.Panel2.SuspendLayout();
        this._contentSplit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this._maxDepthBox).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this._minSizeBox).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this._maxSizeBox).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this._duplicateMinSizeBox).BeginInit();
        this._systemInfoPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this._systemInfoGrid).BeginInit();
        this._progressPanel.SuspendLayout();
        this._resultsPanel.SuspendLayout();
        this._resultsSearchPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this._minSizeFilterBox).BeginInit();
        this._resultsButtonsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this._resultsDataGridView).BeginInit();
        this.SuspendLayout();

        // Fixed header/filter/progress rows; result grid takes remaining space.
        this._mainTable.ColumnCount = 1;
        this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._mainTable.RowCount = 4;
        this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
        this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 285F));
        this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
        this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
        this._mainTable.Padding = new System.Windows.Forms.Padding(8);
        this._mainTable.Controls.Add(this._topPanel, 0, 0);
        this._mainTable.Controls.Add(this._filterPanel, 0, 1);
        this._mainTable.Controls.Add(this._progressPanel, 0, 2);
        this._mainTable.Controls.Add(this._contentSplit, 0, 3);

        // Results and a live, resizable application log share the lower area.
        this._contentSplit.Size = new System.Drawing.Size(1184, 400);
        this._contentSplit.Dock = System.Windows.Forms.DockStyle.Fill;
        this._contentSplit.Orientation = System.Windows.Forms.Orientation.Vertical;
        this._contentSplit.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
        this._contentSplit.Panel2.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
        this._contentSplit.Panel1.Controls.Add(this._resultsPanel);
        this._contentSplit.Panel2.Controls.Add(this._logPanel);

        this._logPanel.ColumnCount = 1;
        this._logPanel.RowCount = 2;
        this._logPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._logPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
        this._logPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._logPanel.Controls.Add(this._logHeader, 0, 0);
        this._logPanel.Controls.Add(this._listBox_log, 0, 1);
        this._logHeader.ColumnCount = 2;
        this._logHeader.RowCount = 1;
        this._logHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._logHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this._logHeader.Dock = System.Windows.Forms.DockStyle.Fill;
        this._logHeader.Controls.Add(this._logTitle, 0, 0);
        this._logHeader.Controls.Add(this._copyLogButton, 1, 0);
        this._logTitle.Text = "Application log";
        this._logTitle.AutoSize = true;
        this._logTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this._copyLogButton.Text = "Copy log";
        this._copyLogButton.AutoSize = true;
        this._copyLogButton.UseVisualStyleBackColor = true;
        this._copyLogButton.Click += new System.EventHandler(this.OnCopyLogClick);
        this._listBox_log.Dock = System.Windows.Forms.DockStyle.Fill;
        this._listBox_log.FormattingEnabled = true;
        this._listBox_log.HorizontalScrollbar = true;
        this._listBox_log.IntegralHeight = false;
        this._listBox_log.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
        this._listBox_log.TabStop = false;
        this._listBox_log.Font = new System.Drawing.Font("Consolas", 8.25F);
        this._listBox_log.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnLogListMouseUp);
        this._listBox_log.DoubleClick += new System.EventHandler(this.OnLogListDoubleClick);

        // Drive/scan/worker row and preset row.
        this._topPanel.ColumnCount = 5;
        this._topPanel.RowCount = 2;
        this._topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this._topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
        this._topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this._topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this._topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
        this._topPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this._topPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this._topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._topPanel.Padding = new System.Windows.Forms.Padding(4);
        this._topPanel.Controls.Add(this._driveLabel, 0, 0);
        this._topPanel.Controls.Add(this._driveComboBox, 1, 0);
        this._topPanel.Controls.Add(this._scanButton, 2, 0);
        this._topPanel.Controls.Add(this._cancelButton, 3, 0);
        this._topPanel.Controls.Add(this._workerControl, 4, 0);
        this._topPanel.Controls.Add(this._presetLabel, 0, 1);
        this._topPanel.Controls.Add(this._presetComboBox, 1, 1);
        this._topPanel.Controls.Add(this._savePresetButton, 2, 1);
        this._topPanel.Controls.Add(this._deletePresetButton, 3, 1);
        this._driveLabel.Text = "Drive / folder:";
        this._driveLabel.AutoSize = true;
        this._driveLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this._driveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this._driveComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this._driveComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 8, 3);
        ConfigureButton(this._scanButton, "Start scan", 92);
        this._scanButton.Click += new System.EventHandler(this.OnScanButtonClick);
        ConfigureButton(this._cancelButton, "Cancel", 80);
        this._cancelButton.Enabled = false;
        this._cancelButton.Click += new System.EventHandler(this.OnCancelButtonClick);
        this._workerControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this._workerControl.MinimumSize = new System.Drawing.Size(280, 36);
        this._workerControl.WorkerCountChanged += new System.Action<int>(this.OnWorkerCountChanged);
        this._presetLabel.Text = "Filter preset:";
        this._presetLabel.AutoSize = true;
        this._presetLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this._presetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this._presetComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this._presetComboBox.SelectedIndexChanged += new System.EventHandler(this.OnPresetComboBoxSelectedIndexChanged);
        ConfigureButton(this._savePresetButton, "Save preset…", 105);
        this._savePresetButton.Click += new System.EventHandler(this.OnSavePresetButtonClick);
        ConfigureButton(this._deletePresetButton, "Delete preset", 105);
        this._deletePresetButton.Click += new System.EventHandler(this.OnDeletePresetButtonClick);

        // Resizable side-by-side filter and system-information areas.
        this._filterPanel.Size = new System.Drawing.Size(1240, 300);
        this._filterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._filterPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
        this._filterPanel.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
        this._filterPanel.Panel2.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);

        var filterGroup = new System.Windows.Forms.GroupBox { Text = "Scan filters", Dock = System.Windows.Forms.DockStyle.Fill, Padding = new System.Windows.Forms.Padding(8) };
        var filterTable = new System.Windows.Forms.TableLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new System.Windows.Forms.Padding(4) };
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        for (int i = 0; i < 4; i++) filterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
        filterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        filterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
        filterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
        filterGroup.Controls.Add(filterTable);
        this._filterPanel.Panel1.Controls.Add(filterGroup);
        this.AddFilterRow(filterTable, 0, "Include extensions:", this._includeExtensionsBox, "All if blank; e.g. .zip, .iso");
        this.AddFilterRow(filterTable, 1, "Exclude extensions:", this._excludeExtensionsBox, "e.g. .tmp, .log, .cache");
        this.AddFilterRow(filterTable, 2, "Include path segments:", this._includePathsBox, "e.g. Downloads, models (blank = all)");
        this.AddFilterRow(filterTable, 3, "Exclude path segments:", this._excludePathsBox, "e.g. bin, obj, .git");

        var sizeFlow = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight, WrapContents = false, AutoScroll = true, Padding = new System.Windows.Forms.Padding(0, 5, 0, 0) };
        sizeFlow.Controls.Add(new System.Windows.Forms.Label { Text = "Max depth:", AutoSize = true, Margin = new System.Windows.Forms.Padding(0, 5, 4, 0) });
        ConfigureNumber(this._maxDepthBox, -1, 50, -1, 70);
        sizeFlow.Controls.Add(this._maxDepthBox);
        sizeFlow.Controls.Add(new System.Windows.Forms.Label { Text = "Min bytes:", AutoSize = true, Margin = new System.Windows.Forms.Padding(10, 5, 4, 0) });
        ConfigureNumber(this._minSizeBox, 0, long.MaxValue, 0, 125);
        sizeFlow.Controls.Add(this._minSizeBox);
        sizeFlow.Controls.Add(new System.Windows.Forms.Label { Text = "Max bytes:", AutoSize = true, Margin = new System.Windows.Forms.Padding(10, 5, 4, 0) });
        ConfigureNumber(this._maxSizeBox, 0, long.MaxValue, long.MaxValue, 190);
        sizeFlow.Controls.Add(this._maxSizeBox);
        filterTable.Controls.Add(sizeFlow, 0, 4);
        filterTable.SetColumnSpan(sizeFlow, 2);

        var optionsFlow = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight, WrapContents = false, AutoScroll = true, Padding = new System.Windows.Forms.Padding(0, 4, 0, 0) };
        ConfigureCheck(this._skipHiddenCheck, "Skip hidden", true);
        ConfigureCheck(this._skipSystemCheck, "Skip system", false);
        ConfigureCheck(this._detectDuplicatesCheck, "Find duplicates (SHA-256)", false);
        optionsFlow.Controls.Add(this._skipHiddenCheck);
        optionsFlow.Controls.Add(this._skipSystemCheck);
        optionsFlow.Controls.Add(this._detectDuplicatesCheck);
        filterTable.Controls.Add(optionsFlow, 0, 5);
        filterTable.SetColumnSpan(optionsFlow, 2);

        var duplicateFlow = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight, WrapContents = false, Padding = new System.Windows.Forms.Padding(0, 4, 0, 0) };
        duplicateFlow.Controls.Add(new System.Windows.Forms.Label { Text = "Minimum duplicate size (bytes):", AutoSize = true, Margin = new System.Windows.Forms.Padding(0, 5, 8, 0) });
        ConfigureNumber(this._duplicateMinSizeBox, 0, long.MaxValue, 1048576, 150);
        duplicateFlow.Controls.Add(this._duplicateMinSizeBox);
        filterTable.Controls.Add(duplicateFlow, 0, 6);
        filterTable.SetColumnSpan(duplicateFlow, 2);

        // Predefine columns: system-info rows are added during form startup.
        this._systemInfoPanel.ColumnCount = 1;
        this._systemInfoPanel.RowCount = 2;
        this._systemInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._systemInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
        this._systemInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._systemInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._systemInfoPanel.Controls.Add(this._refreshSystemInfoButton, 0, 0);
        this._systemInfoPanel.Controls.Add(this._systemInfoGrid, 0, 1);
        this._filterPanel.Panel2.Controls.Add(this._systemInfoPanel);
        this._refreshSystemInfoButton.Text = "Refresh system information";
        this._refreshSystemInfoButton.Dock = System.Windows.Forms.DockStyle.Fill;
        this._refreshSystemInfoButton.UseVisualStyleBackColor = true;
        this._refreshSystemInfoButton.Click += new System.EventHandler(this.OnRefreshSystemInfoClick);
        ConfigureGrid(this._systemInfoGrid);
        this._systemInfoGrid.AutoGenerateColumns = false;
        this._systemInfoGrid.Columns.Add(CreateGridColumn("Category", "Category", 18F));
        this._systemInfoGrid.Columns.Add(CreateGridColumn("Path", "Path", 28F));
        this._systemInfoGrid.Columns.Add(CreateGridColumn("Size", "Size", 14F));
        this._systemInfoGrid.Columns.Add(CreateGridColumn("Description", "Description", 40F));

        // Full-width progress, elapsed time, ETA, counters and current status.
        this._progressPanel.ColumnCount = 1;
        this._progressPanel.RowCount = 3;
        this._progressPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._progressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
        this._progressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
        this._progressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._progressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._progressPanel.Controls.Add(this._progressBar, 0, 0);
        var statsFlow = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight, WrapContents = false, Padding = new System.Windows.Forms.Padding(2, 3, 0, 0) };
        statsFlow.Controls.Add(this._elapsedLabel);
        statsFlow.Controls.Add(this._etaLabel);
        statsFlow.Controls.Add(this._fileCountLabel);
        statsFlow.Controls.Add(this._errorCountLabel);
        this._progressPanel.Controls.Add(statsFlow, 0, 1);
        this._progressPanel.Controls.Add(this._statusLabel, 0, 2);
        this._progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
        this._progressBar.Maximum = 100;
        ConfigureStatusLabel(this._elapsedLabel, "Elapsed: -", 10);
        ConfigureStatusLabel(this._etaLabel, "ETA: -", 30);
        ConfigureStatusLabel(this._fileCountLabel, "Files: 0", 30);
        ConfigureStatusLabel(this._errorCountLabel, "Errors: 0", 30);
        this._statusLabel.Text = "Ready. Select a drive or folder, then start a scan.";
        this._statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this._statusLabel.AutoEllipsis = true;

        // Results grid expands on resize; search and actions stay accessible.
        this._resultsPanel.ColumnCount = 1;
        this._resultsPanel.RowCount = 3;
        this._resultsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._resultsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
        this._resultsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this._resultsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
        this._resultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._resultsPanel.Controls.Add(this._resultsSearchPanel, 0, 0);
        this._resultsPanel.Controls.Add(this._resultsDataGridView, 0, 1);
        this._resultsPanel.Controls.Add(this._resultsButtonsPanel, 0, 2);
        this._resultsSearchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._resultsSearchPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this._resultsSearchPanel.WrapContents = false;
        this._resultsSearchPanel.AutoScroll = true;
        this._resultsSearchPanel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
        this._resultsSearchPanel.Controls.Add(this._searchLabel);
        this._resultsSearchPanel.Controls.Add(this._searchBox);
        this._resultsSearchPanel.Controls.Add(this._minSizeFilterLabel);
        this._resultsSearchPanel.Controls.Add(this._minSizeFilterBox);
        this._searchLabel.Text = "Search results:";
        this._searchLabel.AutoSize = true;
        this._searchLabel.Margin = new System.Windows.Forms.Padding(2, 5, 4, 0);
        this._searchBox.Width = 300;
        this._searchBox.PlaceholderText = "File or folder name / path";
        this._searchBox.TextChanged += new System.EventHandler(this.OnSearchBoxTextChanged);
        this._minSizeFilterLabel.Text = "Minimum size (bytes):";
        this._minSizeFilterLabel.AutoSize = true;
        this._minSizeFilterLabel.Margin = new System.Windows.Forms.Padding(14, 5, 4, 0);
        ConfigureNumber(this._minSizeFilterBox, 0, long.MaxValue, 0, 140);
        this._minSizeFilterBox.ValueChanged += new System.EventHandler(this.OnMinSizeFilterChanged);
        ConfigureGrid(this._resultsDataGridView);
        this._resultsDataGridView.AutoGenerateColumns = false;
        this._resultsDataGridView.Columns.Add(CreateGridColumn("Name", "Name", 16F));
        this._resultsDataGridView.Columns.Add(CreateGridColumn("FullPath", "Full path", 38F));
        this._resultsDataGridView.Columns.Add(CreateGridColumn("SizeBytes", "Size", 10F));
        this._resultsDataGridView.Columns.Add(CreateGridColumn("Extension", "Extension", 8F));
        this._resultsDataGridView.Columns.Add(CreateGridColumn("LastWriteTime", "Modified", 14F, "g"));
        this._resultsDataGridView.Columns.Add(CreateGridColumn("IsDirectory", "Folder", 7F));
        this._resultsDataGridView.Columns.Add(CreateGridColumn("DuplicateGroupId", "Duplicate group", 7F));
        this._resultsDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.OnResultsDataGridViewCellFormatting);
        this._resultsDataGridView.DataSource = this._resultsBindingSource;
        this._resultsButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this._resultsButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this._resultsButtonsPanel.WrapContents = false;
        this._resultsButtonsPanel.Padding = new System.Windows.Forms.Padding(2, 5, 0, 0);
        this._resultsButtonsPanel.Controls.Add(this._exportButton);
        this._resultsButtonsPanel.Controls.Add(this._recycleBinButton);
        this._resultsButtonsPanel.Controls.Add(this._hardDeleteButton);
        ConfigureActionButton(this._exportButton, "Export report…", 130);
        this._exportButton.Click += new System.EventHandler(this.OnExportButtonClick);
        ConfigureActionButton(this._recycleBinButton, "Send to Recycle Bin", 175);
        this._recycleBinButton.Click += new System.EventHandler(this.OnRecycleBinButtonClick);
        ConfigureActionButton(this._hardDeleteButton, "Hard delete…", 130);
        this._hardDeleteButton.Click += new System.EventHandler(this.OnHardDeleteButtonClick);

        // Native Windows Forms palette/font; no custom colors or painting.
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Control;
        this.ClientSize = new System.Drawing.Size(1200, 680);
        this.Controls.Add(this._mainTable);
        this.MinimumSize = new System.Drawing.Size(1050, 650);
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Win11DeTrasher — Disk Space Analyzer & Cleaner";

        ((System.ComponentModel.ISupportInitialize)this._filterPanel).EndInit();
        this._filterPanel.Panel1.ResumeLayout(false);
        this._filterPanel.Panel2.ResumeLayout(false);
        this._filterPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this._contentSplit).EndInit();
        this._contentSplit.Panel1.ResumeLayout(false);
        this._contentSplit.Panel2.ResumeLayout(false);
        this._contentSplit.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this._maxDepthBox).EndInit();
        ((System.ComponentModel.ISupportInitialize)this._minSizeBox).EndInit();
        ((System.ComponentModel.ISupportInitialize)this._maxSizeBox).EndInit();
        ((System.ComponentModel.ISupportInitialize)this._duplicateMinSizeBox).EndInit();
        this._systemInfoPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this._systemInfoGrid).EndInit();
        this._progressPanel.ResumeLayout(false);
        this._resultsPanel.ResumeLayout(false);
        this._resultsSearchPanel.ResumeLayout(false);
        this._resultsSearchPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)this._minSizeFilterBox).EndInit();
        this._resultsButtonsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this._resultsDataGridView).EndInit();
        this.ResumeLayout(false);
    }

    private void AddFilterRow(System.Windows.Forms.TableLayoutPanel table, int row, string labelText, System.Windows.Forms.TextBox textBox, string placeholder)
    {
        var label = new System.Windows.Forms.Label { Text = labelText, AutoSize = true, Anchor = System.Windows.Forms.AnchorStyles.Left, Margin = new System.Windows.Forms.Padding(3, 0, 4, 0) };
        textBox.Dock = System.Windows.Forms.DockStyle.Fill;
        textBox.Margin = new System.Windows.Forms.Padding(3);
        textBox.PlaceholderText = placeholder;
        textBox.TextChanged += this.OnFilterBoxChanged;
        table.Controls.Add(label, 0, row);
        table.Controls.Add(textBox, 1, row);
    }

    private static void ConfigureNumber(System.Windows.Forms.NumericUpDown control, decimal minimum, decimal maximum, decimal value, int width)
    {
        control.Minimum = minimum;
        control.Maximum = maximum;
        control.Value = Math.Clamp(value, minimum, maximum);
        control.Width = width;
        control.ThousandsSeparator = true;
        control.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        control.Margin = new System.Windows.Forms.Padding(2);
        control.Anchor = System.Windows.Forms.AnchorStyles.Left;
    }

    private static void ConfigureCheck(System.Windows.Forms.CheckBox control, string text, bool isChecked)
    {
        control.Text = text;
        control.Checked = isChecked;
        control.AutoSize = true;
        control.UseVisualStyleBackColor = true;
        control.Margin = new System.Windows.Forms.Padding(2, 3, 12, 3);
    }

    private static void ConfigureGrid(System.Windows.Forms.DataGridView grid)
    {
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = System.Drawing.SystemColors.Window;
        grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.Dock = System.Windows.Forms.DockStyle.Fill;
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = true;
        grid.EnableHeadersVisualStyles = true;
    }

    private static System.Windows.Forms.DataGridViewTextBoxColumn CreateGridColumn(string propertyName, string headerText, float fillWeight, string? format = null)
    {
        var column = new System.Windows.Forms.DataGridViewTextBoxColumn
        {
            Name = propertyName,
            DataPropertyName = propertyName,
            HeaderText = headerText,
            AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill,
            FillWeight = fillWeight,
            SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic,
            MinimumWidth = 65
        };
        if (format is not null) column.DefaultCellStyle.Format = format;
        return column;
    }

    private static void ConfigureStatusLabel(System.Windows.Forms.Label label, string text, int leftMargin)
    {
        label.Text = text;
        label.AutoSize = true;
        label.Anchor = System.Windows.Forms.AnchorStyles.Left;
        label.Margin = new System.Windows.Forms.Padding(leftMargin, 2, 20, 2);
    }

    private static void ConfigureButton(System.Windows.Forms.Button button, string text, int width)
    {
        button.Text = text;
        button.AutoSize = true;
        button.MinimumSize = new System.Drawing.Size(width, 30);
        button.UseVisualStyleBackColor = true;
        button.Margin = new System.Windows.Forms.Padding(4, 2, 8, 2);
    }

    private static void ConfigureActionButton(System.Windows.Forms.Button button, string text, int width)
    {
        button.Text = text;
        button.Width = width;
        button.Height = 30;
        button.UseVisualStyleBackColor = true;
        button.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
    }

    #endregion

    #nullable disable
    private System.Windows.Forms.TableLayoutPanel _mainTable;
    private System.Windows.Forms.TableLayoutPanel _topPanel;
    private System.Windows.Forms.Label _driveLabel;
    private System.Windows.Forms.ComboBox _driveComboBox;
    private System.Windows.Forms.Button _scanButton;
    private System.Windows.Forms.Button _cancelButton;
    private Controls.WorkerControl _workerControl;
    private System.Windows.Forms.Label _presetLabel;
    private System.Windows.Forms.ComboBox _presetComboBox;
    private System.Windows.Forms.Button _savePresetButton;
    private System.Windows.Forms.Button _deletePresetButton;
    private System.Windows.Forms.SplitContainer _filterPanel;
    private System.Windows.Forms.SplitContainer _contentSplit;
    private System.Windows.Forms.TextBox _includeExtensionsBox;
    private System.Windows.Forms.TextBox _excludeExtensionsBox;
    private System.Windows.Forms.TextBox _includePathsBox;
    private System.Windows.Forms.TextBox _excludePathsBox;
    private System.Windows.Forms.NumericUpDown _maxDepthBox;
    private System.Windows.Forms.NumericUpDown _minSizeBox;
    private System.Windows.Forms.NumericUpDown _maxSizeBox;
    private System.Windows.Forms.CheckBox _skipHiddenCheck;
    private System.Windows.Forms.CheckBox _skipSystemCheck;
    private System.Windows.Forms.CheckBox _detectDuplicatesCheck;
    private System.Windows.Forms.NumericUpDown _duplicateMinSizeBox;
    private System.Windows.Forms.TableLayoutPanel _systemInfoPanel;
    private System.Windows.Forms.Button _refreshSystemInfoButton;
    private System.Windows.Forms.DataGridView _systemInfoGrid;
    private System.Windows.Forms.TableLayoutPanel _progressPanel;
    private System.Windows.Forms.ProgressBar _progressBar;
    private System.Windows.Forms.Label _elapsedLabel;
    private System.Windows.Forms.Label _etaLabel;
    private System.Windows.Forms.Label _fileCountLabel;
    private System.Windows.Forms.Label _errorCountLabel;
    private System.Windows.Forms.Label _statusLabel;
    private System.Windows.Forms.TableLayoutPanel _resultsPanel;
    private System.Windows.Forms.FlowLayoutPanel _resultsSearchPanel;
    private System.Windows.Forms.Label _searchLabel;
    private System.Windows.Forms.TextBox _searchBox;
    private System.Windows.Forms.Label _minSizeFilterLabel;
    private System.Windows.Forms.NumericUpDown _minSizeFilterBox;
    private Controls.SortableDataGridView _resultsDataGridView;
    private System.Windows.Forms.FlowLayoutPanel _resultsButtonsPanel;
    private System.Windows.Forms.Button _exportButton;
    private System.Windows.Forms.Button _recycleBinButton;
    private System.Windows.Forms.Button _hardDeleteButton;
    private System.Windows.Forms.TableLayoutPanel _logPanel;
    private System.Windows.Forms.TableLayoutPanel _logHeader;
    private System.Windows.Forms.Label _logTitle;
    private System.Windows.Forms.Button _copyLogButton;
    private System.Windows.Forms.ListBox _listBox_log;
    #nullable restore
}
