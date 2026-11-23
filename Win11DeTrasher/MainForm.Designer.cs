namespace Win11DeTrasher
{
    partial class MainForm
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._mainPanel = new System.Windows.Forms.Panel();
            this._topPanel = new System.Windows.Forms.Panel();
            this._topFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._driveLabel = new System.Windows.Forms.Label();
            this._driveComboBox = new System.Windows.Forms.ComboBox();
            this._scanButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._workerControl = new Controls.WorkerControl();
            this._presetLabel = new System.Windows.Forms.Label();
            this._presetComboBox = new System.Windows.Forms.ComboBox();
            this._savePresetButton = new System.Windows.Forms.Button();
            this._deletePresetButton = new System.Windows.Forms.Button();
            this._filterPanel = new System.Windows.Forms.Panel();
            this._filterFlow = new System.Windows.Forms.FlowLayoutPanel();
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
            this._rightPanel = new System.Windows.Forms.Panel();
            this._rightSplit = new System.Windows.Forms.SplitContainer();
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
            this._resultsPanel = new System.Windows.Forms.Panel();
            this._resultsSplit = new System.Windows.Forms.SplitContainer();
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
            this._mainPanel.SuspendLayout();
            this._topPanel.SuspendLayout();
            this._topFlow.SuspendLayout();
            this._filterPanel.SuspendLayout();
            this._filterFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._maxDepthBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._minSizeBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._maxSizeBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._duplicateMinSizeBox).BeginInit();
            this._rightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._rightSplit).BeginInit();
            this._rightSplit.Panel1.SuspendLayout();
            this._rightSplit.Panel2.SuspendLayout();
            this._rightSplit.SuspendLayout();
            this._systemInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._systemInfoGrid).BeginInit();
            this._progressPanel.SuspendLayout();
            this._progressPanel.PerformLayout();
            this._resultsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._resultsSplit).BeginInit();
            this._resultsSplit.Panel1.SuspendLayout();
            this._resultsSplit.Panel2.SuspendLayout();
            this._resultsSplit.SuspendLayout();
            this._resultsSearchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._minSizeFilterBox).BeginInit();
            this._resultsButtonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._resultsDataGridView).BeginInit();
            this.SuspendLayout();

            //
            // _mainPanel
            //
            this._mainPanel.Controls.Add(this._topPanel);
            this._mainPanel.Controls.Add(this._filterPanel);
            this._mainPanel.Controls.Add(this._rightPanel);
            this._mainPanel.Controls.Add(this._progressPanel);
            this._mainPanel.Controls.Add(this._resultsPanel);
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 0);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(1100, 800);
            this._mainPanel.TabIndex = 0;

            //
            // _topPanel
            //
            this._topPanel.Controls.Add(this._topFlow);
            this._topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._topPanel.Location = new System.Drawing.Point(0, 0);
            this._topPanel.Name = "_topPanel";
            this._topPanel.Size = new System.Drawing.Size(1100, 56);
            this._topPanel.TabIndex = 0;

            //
            // _topFlow
            //
            this._topFlow.Controls.Add(this._driveLabel);
            this._topFlow.Controls.Add(this._driveComboBox);
            this._topFlow.Controls.Add(this._scanButton);
            this._topFlow.Controls.Add(this._cancelButton);
            this._topFlow.Controls.Add(this._workerControl);
            this._topFlow.Controls.Add(this._presetLabel);
            this._topFlow.Controls.Add(this._presetComboBox);
            this._topFlow.Controls.Add(this._savePresetButton);
            this._topFlow.Controls.Add(this._deletePresetButton);
            this._topFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._topFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._topFlow.Location = new System.Drawing.Point(0, 0);
            this._topFlow.Name = "_topFlow";
            this._topFlow.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this._topFlow.Size = new System.Drawing.Size(1100, 56);
            this._topFlow.TabIndex = 0;
            this._topFlow.WrapContents = false;

            //
            // _driveLabel
            //
            this._driveLabel.AutoSize = true;
            this._driveLabel.Location = new System.Drawing.Point(11, 6);
            this._driveLabel.Name = "_driveLabel";
            this._driveLabel.Size = new System.Drawing.Size(54, 20);
            this._driveLabel.TabIndex = 0;
            this._driveLabel.Text = "Drive:";

            //
            // _driveComboBox
            //
            this._driveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._driveComboBox.Location = new System.Drawing.Point(71, 3);
            this._driveComboBox.Name = "_driveComboBox";
            this._driveComboBox.Size = new System.Drawing.Size(280, 28);
            this._driveComboBox.TabIndex = 1;

            //
            // _scanButton
            //
            this._scanButton.Location = new System.Drawing.Point(357, 3);
            this._scanButton.Name = "_scanButton";
            this._scanButton.Size = new System.Drawing.Size(90, 32);
            this._scanButton.TabIndex = 2;
            this._scanButton.Text = "Scan";
            this._scanButton.UseVisualStyleBackColor = true;
            this._scanButton.Click += new System.EventHandler(this.OnScanButtonClick);

            //
            // _cancelButton
            //
            this._cancelButton.Enabled = false;
            this._cancelButton.Location = new System.Drawing.Point(453, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(90, 32);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.OnCancelButtonClick);

            //
            // _workerControl
            //
            this._workerControl.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._workerControl.Location = new System.Drawing.Point(549, 3);
            this._workerControl.Name = "_workerControl";
            this._workerControl.Size = new System.Drawing.Size(540, 30);
            this._workerControl.TabIndex = 4;
            this._workerControl.WorkerCountChanged += new System.Action<int>(this.OnWorkerCountChanged);

            //
            // _presetLabel
            //
            this._presetLabel.AutoSize = true;
            this._presetLabel.Location = new System.Drawing.Point(11, 41);
            this._presetLabel.Name = "_presetLabel";
            this._presetLabel.Size = new System.Drawing.Size(58, 20);
            this._presetLabel.TabIndex = 5;
            this._presetLabel.Text = "Preset:";

            //
            // _presetComboBox
            //
            this._presetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._presetComboBox.Location = new System.Drawing.Point(75, 38);
            this._presetComboBox.Name = "_presetComboBox";
            this._presetComboBox.Size = new System.Drawing.Size(200, 28);
            this._presetComboBox.TabIndex = 6;
            this._presetComboBox.SelectedIndexChanged += new System.EventHandler(this.OnPresetComboBoxSelectedIndexChanged);

            //
            // _savePresetButton
            //
            this._savePresetButton.Location = new System.Drawing.Point(281, 38);
            this._savePresetButton.Name = "_savePresetButton";
            this._savePresetButton.Size = new System.Drawing.Size(100, 30);
            this._savePresetButton.TabIndex = 7;
            this._savePresetButton.Text = "Save Preset";
            this._savePresetButton.UseVisualStyleBackColor = true;
            this._savePresetButton.Click += new System.EventHandler(this.OnSavePresetButtonClick);

            //
            // _deletePresetButton
            //
            this._deletePresetButton.Location = new System.Drawing.Point(387, 38);
            this._deletePresetButton.Name = "_deletePresetButton";
            this._deletePresetButton.Size = new System.Drawing.Size(100, 30);
            this._deletePresetButton.TabIndex = 8;
            this._deletePresetButton.Text = "Delete Preset";
            this._deletePresetButton.UseVisualStyleBackColor = true;
            this._deletePresetButton.Click += new System.EventHandler(this.OnDeletePresetButtonClick);

            //
            // _filterPanel
            //
            this._filterPanel.Controls.Add(this._filterFlow);
            this._filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._filterPanel.Location = new System.Drawing.Point(0, 56);
            this._filterPanel.Name = "_filterPanel";
            this._filterPanel.Size = new System.Drawing.Size(420, 200);
            this._filterPanel.TabIndex = 1;

            //
            // _filterFlow
            //
            this._filterFlow.Controls.Add(this._includeExtensionsBox);
            this._filterFlow.Controls.Add(this._excludeExtensionsBox);
            this._filterFlow.Controls.Add(this._includePathsBox);
            this._filterFlow.Controls.Add(this._excludePathsBox);
            this._filterFlow.Controls.Add(this._maxDepthBox);
            this._filterFlow.Controls.Add(this._minSizeBox);
            this._filterFlow.Controls.Add(this._maxSizeBox);
            this._filterFlow.Controls.Add(this._skipHiddenCheck);
            this._filterFlow.Controls.Add(this._skipSystemCheck);
            this._filterFlow.Controls.Add(this._detectDuplicatesCheck);
            this._filterFlow.Controls.Add(this._duplicateMinSizeBox);
            this._filterFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._filterFlow.Location = new System.Drawing.Point(0, 0);
            this._filterFlow.Name = "_filterFlow";
            this._filterFlow.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this._filterFlow.Size = new System.Drawing.Size(420, 200);
            this._filterFlow.TabIndex = 1;
            this._filterFlow.WrapContents = false;

            //
            // _includeExtensionsBox
            //
            this._includeExtensionsBox.Location = new System.Drawing.Point(120, 15);
            this._includeExtensionsBox.Name = "_includeExtensionsBox";
            this._includeExtensionsBox.PlaceholderText = "e.g. .dll, .onnx, .so";
            this._includeExtensionsBox.Size = new System.Drawing.Size(290, 27);
            this._includeExtensionsBox.TabIndex = 0;
            this._includeExtensionsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);

            //
            // _excludeExtensionsBox
            //
            this._excludeExtensionsBox.Location = new System.Drawing.Point(120, 48);
            this._excludeExtensionsBox.Name = "_excludeExtensionsBox";
            this._excludeExtensionsBox.PlaceholderText = "e.g. .tmp, .log, .cache";
            this._excludeExtensionsBox.Size = new System.Drawing.Size(290, 27);
            this._excludeExtensionsBox.TabIndex = 1;
            this._excludeExtensionsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);

            //
            // _includePathsBox
            //
            this._includePathsBox.Location = new System.Drawing.Point(120, 81);
            this._includePathsBox.Name = "_includePathsBox";
            this._includePathsBox.PlaceholderText = "e.g. .nuget, onnx, models";
            this._includePathsBox.Size = new System.Drawing.Size(290, 27);
            this._includePathsBox.TabIndex = 2;
            this._includePathsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);

            //
            // _excludePathsBox
            //
            this._excludePathsBox.Location = new System.Drawing.Point(120, 114);
            this._excludePathsBox.Name = "_excludePathsBox";
            this._excludePathsBox.PlaceholderText = "e.g. bin, obj, .git";
            this._excludePathsBox.Size = new System.Drawing.Size(290, 27);
            this._excludePathsBox.TabIndex = 3;
            this._excludePathsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);

            //
            // _maxDepthBox
            //
            this._maxDepthBox.Location = new System.Drawing.Point(120, 147);
            this._maxDepthBox.Name = "_maxDepthBox";
            this._maxDepthBox.Size = new System.Drawing.Size(120, 27);
            this._maxDepthBox.TabIndex = 4;
            this._maxDepthBox.Value = new decimal(new int[] { -1, 0, 0, 0 });
            this._maxDepthBox.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            this._maxDepthBox.Maximum = new decimal(new int[] { 50, 0, 0, 0 });

            //
            // _minSizeBox
            //
            this._minSizeBox.Location = new System.Drawing.Point(250, 147);
            this._minSizeBox.Name = "_minSizeBox";
            this._minSizeBox.Size = new System.Drawing.Size(120, 27);
            this._minSizeBox.TabIndex = 5;
            this._minSizeBox.Maximum = new decimal(new int[] { 0, 0, 0, 0 });

            //
            // _maxSizeBox
            //
            this._maxSizeBox.Location = new System.Drawing.Point(380, 147);
            this._maxSizeBox.Name = "_maxSizeBox";
            this._maxSizeBox.Size = new System.Drawing.Size(120, 27);
            this._maxSizeBox.TabIndex = 6;
            this._maxSizeBox.Maximum = new decimal(new int[] { 0, 0, 0, 0 });

            //
            // _skipHiddenCheck
            //
            this._skipHiddenCheck.AutoSize = true;
            this._skipHiddenCheck.Checked = true;
            this._skipHiddenCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this._skipHiddenCheck.Location = new System.Drawing.Point(10, 180);
            this._skipHiddenCheck.Name = "_skipHiddenCheck";
            this._skipHiddenCheck.Size = new System.Drawing.Size(170, 24);
            this._skipHiddenCheck.TabIndex = 7;
            this._skipHiddenCheck.Text = "Skip hidden files/dirs";
            this._skipHiddenCheck.UseVisualStyleBackColor = true;

            //
            // _skipSystemCheck
            //
            this._skipSystemCheck.AutoSize = true;
            this._skipSystemCheck.Location = new System.Drawing.Point(190, 180);
            this._skipSystemCheck.Name = "_skipSystemCheck";
            this._skipSystemCheck.Size = new System.Drawing.Size(170, 24);
            this._skipSystemCheck.TabIndex = 8;
            this._skipSystemCheck.Text = "Skip system files/dirs";
            this._skipSystemCheck.UseVisualStyleBackColor = true;

            //
            // _detectDuplicatesCheck
            //
            this._detectDuplicatesCheck.AutoSize = true;
            this._detectDuplicatesCheck.Location = new System.Drawing.Point(370, 180);
            this._detectDuplicatesCheck.Name = "_detectDuplicatesCheck";
            this._detectDuplicatesCheck.Size = new System.Drawing.Size(210, 24);
            this._detectDuplicatesCheck.TabIndex = 9;
            this._detectDuplicatesCheck.Text = "Detect duplicates (SHA256)";
            this._detectDuplicatesCheck.UseVisualStyleBackColor = true;

            //
            // _duplicateMinSizeBox
            //
            this._duplicateMinSizeBox.Location = new System.Drawing.Point(120, 210);
            this._duplicateMinSizeBox.Name = "_duplicateMinSizeBox";
            this._duplicateMinSizeBox.Size = new System.Drawing.Size(120, 27);
            this._duplicateMinSizeBox.TabIndex = 10;
            this._duplicateMinSizeBox.Value = new decimal(new int[] { 1048576, 0, 0, 0 });
            this._duplicateMinSizeBox.Minimum = new decimal(new int[] { 1048576, 0, 0, 0 });

            //
            // _rightPanel
            //
            this._rightPanel.Controls.Add(this._rightSplit);
            this._rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this._rightPanel.Location = new System.Drawing.Point(640, 56);
            this._rightPanel.Name = "_rightPanel";
            this._rightPanel.Size = new System.Drawing.Size(460, 424);
            this._rightPanel.TabIndex = 2;

            //
            // _rightSplit
            //
            this._rightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightSplit.Location = new System.Drawing.Point(0, 0);
            this._rightSplit.Name = "_rightSplit";
            this._rightSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this._rightSplit.Panel1.Controls.Add(this._systemInfoPanel);
            this._rightSplit.Panel2.Controls.Add(this._progressPanel);
            this._rightSplit.Size = new System.Drawing.Size(460, 424);
            this._rightSplit.SplitterDistance = 150;
            this._rightSplit.TabIndex = 0;

            //
            // _systemInfoPanel
            //
            this._systemInfoPanel.Controls.Add(this._refreshSystemInfoButton, 0, 0);
            this._systemInfoPanel.Controls.Add(this._systemInfoGrid, 0, 1);
            this._systemInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._systemInfoPanel.Location = new System.Drawing.Point(0, 0);
            this._systemInfoPanel.Name = "_systemInfoPanel";
            this._systemInfoPanel.RowCount = 2;
            this._systemInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this._systemInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._systemInfoPanel.Size = new System.Drawing.Size(460, 147);
            this._systemInfoPanel.TabIndex = 0;

            //
            // _refreshSystemInfoButton
            //
            this._refreshSystemInfoButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._refreshSystemInfoButton.Location = new System.Drawing.Point(3, 3);
            this._refreshSystemInfoButton.Name = "_refreshSystemInfoButton";
            this._refreshSystemInfoButton.Size = new System.Drawing.Size(454, 29);
            this._refreshSystemInfoButton.TabIndex = 0;
            this._refreshSystemInfoButton.Text = "Refresh System Info";
            this._refreshSystemInfoButton.UseVisualStyleBackColor = true;
            this._refreshSystemInfoButton.Click += new System.EventHandler(this.OnRefreshSystemInfoClick);

            //
            // _systemInfoGrid
            //
            this._systemInfoGrid.AllowUserToAddRows = false;
            this._systemInfoGrid.AllowUserToDeleteRows = false;
            this._systemInfoGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._systemInfoGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._systemInfoGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._systemInfoGrid.Location = new System.Drawing.Point(3, 38);
            this._systemInfoGrid.Name = "_systemInfoGrid";
            this._systemInfoGrid.ReadOnly = true;
            this._systemInfoGrid.Size = new System.Drawing.Size(454, 106);
            this._systemInfoGrid.TabIndex = 1;

            //
            // _progressPanel
            //
            this._progressPanel.Controls.Add(this._progressBar, 0, 0);
            this._progressPanel.Controls.Add(this._elapsedLabel, 0, 1);
            this._progressPanel.Controls.Add(this._etaLabel, 1, 1);
            this._progressPanel.Controls.Add(this._fileCountLabel, 2, 1);
            this._progressPanel.Controls.Add(this._errorCountLabel, 3, 1);
            this._progressPanel.Controls.Add(this._statusLabel, 0, 2);
            this._progressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._progressPanel.Location = new System.Drawing.Point(0, 153);
            this._progressPanel.Name = "_progressPanel";
            this._progressPanel.RowCount = 3;
            this._progressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._progressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this._progressPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._progressPanel.Size = new System.Drawing.Size(460, 268);
            this._progressPanel.TabIndex = 1;

            //
            // _progressBar
            //
            this._progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this._progressBar.Location = new System.Drawing.Point(3, 3);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(454, 24);
            this._progressBar.TabIndex = 0;

            //
            // _elapsedLabel
            //
            this._elapsedLabel.AutoSize = true;
            this._elapsedLabel.Location = new System.Drawing.Point(3, 33);
            this._elapsedLabel.Name = "_elapsedLabel";
            this._elapsedLabel.Size = new System.Drawing.Size(45, 20);
            this._elapsedLabel.TabIndex = 1;
            this._elapsedLabel.Text = "Ready.";
            this._elapsedLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;

            //
            // _etaLabel
            //
            this._etaLabel.AutoSize = true;
            this._etaLabel.Location = new System.Drawing.Point(230, 33);
            this._etaLabel.Name = "_etaLabel";
            this._etaLabel.Size = new System.Drawing.Size(45, 20);
            this._etaLabel.TabIndex = 2;
            this._etaLabel.Text = "ETA: -";
            this._etaLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;

            //
            // _fileCountLabel
            //
            this._fileCountLabel.AutoSize = true;
            this._fileCountLabel.Location = new System.Drawing.Point(433, 33);
            this._fileCountLabel.Name = "_fileCountLabel";
            this._fileCountLabel.Size = new System.Drawing.Size(45, 20);
            this._fileCountLabel.TabIndex = 3;
            this._fileCountLabel.Text = "Files: 0";
            this._fileCountLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;

            //
            // _errorCountLabel
            //
            this._errorCountLabel.AutoSize = true;
            this._errorCountLabel.Location = new System.Drawing.Point(300, 33);
            this._errorCountLabel.Name = "_errorCountLabel";
            this._errorCountLabel.Size = new System.Drawing.Size(45, 20);
            this._errorCountLabel.TabIndex = 4;
            this._errorCountLabel.Text = "Errors: 0";
            this._errorCountLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;

            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Location = new System.Drawing.Point(3, 55);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(45, 20);
            this._statusLabel.TabIndex = 5;
            this._statusLabel.Text = "Ready.";
            this._statusLabel.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            //
            // _resultsPanel
            //
            this._resultsPanel.Controls.Add(this._resultsSplit);
            this._resultsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._resultsPanel.Location = new System.Drawing.Point(0, 480);
            this._resultsPanel.Name = "_resultsPanel";
            this._resultsPanel.Size = new System.Drawing.Size(1100, 320);
            this._resultsPanel.TabIndex = 3;

            //
            // _resultsSplit
            //
            this._resultsSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsSplit.Location = new System.Drawing.Point(0, 0);
            this._resultsSplit.Name = "_resultsSplit";
            this._resultsSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this._resultsSplit.Panel1.Controls.Add(this._resultsSearchPanel);
            this._resultsSplit.Panel2.Controls.Add(this._resultsDataGridView);
            this._resultsSplit.Size = new System.Drawing.Size(1100, 320);
            this._resultsSplit.SplitterDistance = 35;
            this._resultsSplit.TabIndex = 0;

            //
            // _resultsSearchPanel
            //
            this._resultsSearchPanel.Controls.Add(this._searchLabel);
            this._resultsSearchPanel.Controls.Add(this._searchBox);
            this._resultsSearchPanel.Controls.Add(this._minSizeFilterLabel);
            this._resultsSearchPanel.Controls.Add(this._minSizeFilterBox);
            this._resultsSearchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsSearchPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this._resultsSearchPanel.Padding = new System.Windows.Forms.Padding(5);
            this._resultsSearchPanel.Size = new System.Drawing.Size(1100, 29);
            this._resultsSearchPanel.TabIndex = 0;
            this._resultsSearchPanel.WrapContents = false;

            //
            // _searchLabel
            //
            this._searchLabel.AutoSize = true;
            this._searchLabel.Location = new System.Drawing.Point(8, 4);
            this._searchLabel.Name = "_searchLabel";
            this._searchLabel.Size = new System.Drawing.Size(58, 20);
            this._searchLabel.TabIndex = 0;
            this._searchLabel.Text = "Search:";

            //
            // _searchBox
            //
            this._searchBox.Location = new System.Drawing.Point(72, 4);
            this._searchBox.Name = "_searchBox";
            this._searchBox.Size = new System.Drawing.Size(300, 27);
            this._searchBox.TabIndex = 1;
            this._searchBox.TextChanged += new System.EventHandler(this.OnSearchBoxTextChanged);

            //
            // _minSizeFilterLabel
            //
            this._minSizeFilterLabel.AutoSize = true;
            this._minSizeFilterLabel.Location = new System.Drawing.Point(382, 4);
            this._minSizeFilterLabel.Name = "_minSizeFilterLabel";
            this._minSizeFilterLabel.Size = new System.Drawing.Size(80, 20);
            this._minSizeFilterLabel.TabIndex = 2;
            this._minSizeFilterLabel.Text = "Min size:";

            //
            // _minSizeFilterBox
            //
            this._minSizeFilterBox.Location = new System.Drawing.Point(468, 4);
            this._minSizeFilterBox.Name = "_minSizeFilterBox";
            this._minSizeFilterBox.Size = new System.Drawing.Size(150, 27);
            this._minSizeFilterBox.TabIndex = 3;
            this._minSizeFilterBox.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this._minSizeFilterBox.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            this._minSizeFilterBox.ValueChanged += new System.EventHandler(this.OnMinSizeFilterChanged);

            //
            // _resultsDataGridView
            //
            this._resultsDataGridView.AllowUserToAddRows = false;
            this._resultsDataGridView.AllowUserToDeleteRows = false;
            this._resultsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._resultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._resultsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsDataGridView.Location = new System.Drawing.Point(0, 35);
            this._resultsDataGridView.MultiSelect = true;
            this._resultsDataGridView.Name = "_resultsDataGridView";
            this._resultsDataGridView.ReadOnly = true;
            this._resultsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._resultsDataGridView.Size = new System.Drawing.Size(1100, 280);
            this._resultsDataGridView.TabIndex = 4;

            //
            // _resultsButtonsPanel
            //
            this._resultsButtonsPanel.Controls.Add(this._exportButton);
            this._resultsButtonsPanel.Controls.Add(this._recycleBinButton);
            this._resultsButtonsPanel.Controls.Add(this._hardDeleteButton);
            this._resultsButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._resultsButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this._resultsButtonsPanel.Padding = new System.Windows.Forms.Padding(5);
            this._resultsButtonsPanel.Size = new System.Drawing.Size(1100, 40);
            this._resultsButtonsPanel.TabIndex = 5;
            this._resultsButtonsPanel.WrapContents = false;

            //
            // _exportButton
            //
            this._exportButton.Location = new System.Drawing.Point(8, 5);
            this._exportButton.Name = "_exportButton";
            this._exportButton.Size = new System.Drawing.Size(120, 30);
            this._exportButton.TabIndex = 5;
            this._exportButton.Text = "Export Report...";
            this._exportButton.UseVisualStyleBackColor = true;
            this._exportButton.Click += new System.EventHandler(this.OnExportButtonClick);

            //
            // _recycleBinButton
            //
            this._recycleBinButton.Location = new System.Drawing.Point(134, 5);
            this._recycleBinButton.Name = "_recycleBinButton";
            this._recycleBinButton.Size = new System.Drawing.Size(160, 30);
            this._recycleBinButton.TabIndex = 6;
            this._recycleBinButton.Text = "Send to Recycle Bin";
            this._recycleBinButton.UseVisualStyleBackColor = true;
            this._recycleBinButton.Click += new System.EventHandler(this.OnRecycleBinButtonClick);

            //
            // _hardDeleteButton
            //
            this._hardDeleteButton.Location = new System.Drawing.Point(300, 5);
            this._hardDeleteButton.Name = "_hardDeleteButton";
            this._hardDeleteButton.Size = new System.Drawing.Size(160, 30);
            this._hardDeleteButton.TabIndex = 7;
            this._hardDeleteButton.Text = "Hard Delete (final)";
            this._hardDeleteButton.UseVisualStyleBackColor = true;
            this._hardDeleteButton.Click += new System.EventHandler(this.OnHardDeleteButtonClick);

            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1100, 800);
            this.Controls.Add(this._mainPanel);
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Win11DeTrasher — Disk Space Analyzer & Cleaner";
            this._mainPanel.ResumeLayout(false);
            this._topPanel.ResumeLayout(false);
            this._topPanel.PerformLayout();
            this._topFlow.ResumeLayout(false);
            this._topFlow.PerformLayout();
            this._filterPanel.ResumeLayout(false);
            this._filterFlow.ResumeLayout(false);
            this._filterFlow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._maxDepthBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._minSizeBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._maxSizeBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._duplicateMinSizeBox).EndInit();
            this._rightPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._rightSplit).EndInit();
            this._rightSplit.Panel1.ResumeLayout(false);
            this._rightSplit.Panel2.ResumeLayout(false);
            this._rightSplit.ResumeLayout(false);
            this._systemInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._systemInfoGrid).EndInit();
            this._progressPanel.ResumeLayout(false);
            this._progressPanel.PerformLayout();
            this._resultsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._resultsSplit).EndInit();
            this._resultsSplit.Panel1.ResumeLayout(false);
            this._resultsSplit.Panel2.ResumeLayout(false);
            this._resultsSplit.ResumeLayout(false);
            this._resultsSearchPanel.ResumeLayout(false);
            this._resultsSearchPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._minSizeFilterBox).EndInit();
            this._resultsButtonsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._resultsDataGridView).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
        #nullable disable

        private System.Windows.Forms.Panel _mainPanel;
        private System.Windows.Forms.Panel _topPanel;
        private System.Windows.Forms.FlowLayoutPanel _topFlow;
        private System.Windows.Forms.Label _driveLabel;
        private System.Windows.Forms.ComboBox _driveComboBox;
        private System.Windows.Forms.Button _scanButton;
        private System.Windows.Forms.Button _cancelButton;
        private Controls.WorkerControl _workerControl;
        private System.Windows.Forms.Label _presetLabel;
        private System.Windows.Forms.ComboBox _presetComboBox;
        private System.Windows.Forms.Button _savePresetButton;
        private System.Windows.Forms.Button _deletePresetButton;
        private System.Windows.Forms.Panel _filterPanel;
        private System.Windows.Forms.FlowLayoutPanel _filterFlow;
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
        private System.Windows.Forms.Panel _rightPanel;
        private System.Windows.Forms.SplitContainer _rightSplit;
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
        private System.Windows.Forms.Panel _resultsPanel;
        private System.Windows.Forms.SplitContainer _resultsSplit;
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
    }
}