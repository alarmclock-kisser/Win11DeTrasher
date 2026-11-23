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
            this._mainTable = new System.Windows.Forms.TableLayoutPanel();
            this._topPanel = new System.Windows.Forms.Panel();
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
            this._progressPanel = new System.Windows.Forms.Panel();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._elapsedLabel = new System.Windows.Forms.Label();
            this._etaLabel = new System.Windows.Forms.Label();
            this._fileCountLabel = new System.Windows.Forms.Label();
            this._errorCountLabel = new System.Windows.Forms.Label();
            this._statusLabel = new System.Windows.Forms.Label();
            this._resultsPanel = new System.Windows.Forms.Panel();
            this._searchBox = new System.Windows.Forms.TextBox();
            this._searchLabel = new System.Windows.Forms.Label();
            this._minSizeFilterBox = new System.Windows.Forms.NumericUpDown();
            this._minSizeFilterLabel = new System.Windows.Forms.Label();
            this._resultsDataGridView = new Controls.SortableDataGridView();
            this._exportButton = new System.Windows.Forms.Button();
            this._recycleBinButton = new System.Windows.Forms.Button();
            this._hardDeleteButton = new System.Windows.Forms.Button();
            this._systemInfoPanel = new System.Windows.Forms.Panel();
            this._systemInfoGrid = new System.Windows.Forms.DataGridView();
            this._refreshSystemInfoButton = new System.Windows.Forms.Button();
            this._mainTable.SuspendLayout();
            this._topPanel.SuspendLayout();
            this._filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._maxDepthBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._minSizeBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._maxSizeBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._duplicateMinSizeBox).BeginInit();
            this._progressPanel.SuspendLayout();
            this._resultsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._minSizeFilterBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._resultsDataGridView).BeginInit();
            this._systemInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._systemInfoGrid).BeginInit();
            this.SuspendLayout();
            // 
            // _mainTable
            // 
            this._mainTable.ColumnCount = 2;
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this._mainTable.Controls.Add(this._topPanel, 0, 0);
            this._mainTable.Controls.Add(this._filterPanel, 0, 1);
            this._mainTable.Controls.Add(this._progressPanel, 0, 2);
            this._mainTable.Controls.Add(this._resultsPanel, 0, 3);
            this._mainTable.Controls.Add(this._systemInfoPanel, 1, 1);
            this._mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainTable.Location = new System.Drawing.Point(0, 0);
            this._mainTable.Name = "_mainTable";
            this._mainTable.RowCount = 4;
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this._mainTable.Size = new System.Drawing.Size(1100, 800);
            this._mainTable.TabIndex = 0;
            // 
            // _topPanel
            // 
            this._topPanel.Controls.Add(this._driveLabel);
            this._topPanel.Controls.Add(this._driveComboBox);
            this._topPanel.Controls.Add(this._scanButton);
            this._topPanel.Controls.Add(this._cancelButton);
            this._topPanel.Controls.Add(this._workerControl);
            this._topPanel.Controls.Add(this._presetLabel);
            this._topPanel.Controls.Add(this._presetComboBox);
            this._topPanel.Controls.Add(this._savePresetButton);
            this._topPanel.Controls.Add(this._deletePresetButton);
            this._topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._topPanel.Location = new System.Drawing.Point(3, 3);
            this._topPanel.Name = "_topPanel";
            this._topPanel.Size = new System.Drawing.Size(651, 54);
            this._topPanel.TabIndex = 0;
            // 
            // _driveLabel
            // 
            this._driveLabel.AutoSize = true;
            this._driveLabel.Location = new System.Drawing.Point(10, 9);
            this._driveLabel.Name = "_driveLabel";
            this._driveLabel.Size = new System.Drawing.Size(54, 20);
            this._driveLabel.TabIndex = 0;
            this._driveLabel.Text = "Drive:";
            // 
            // _driveComboBox
            // 
            this._driveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._driveComboBox.Location = new System.Drawing.Point(70, 6);
            this._driveComboBox.Name = "_driveComboBox";
            this._driveComboBox.Size = new System.Drawing.Size(280, 28);
            this._driveComboBox.TabIndex = 1;
            // 
            // _scanButton
            // 
            this._scanButton.Location = new System.Drawing.Point(360, 4);
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
            this._cancelButton.Location = new System.Drawing.Point(455, 4);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(90, 32);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.OnCancelButtonClick);
            // 
            // _workerControl
            // 
            this._workerControl.Location = new System.Drawing.Point(550, 0);
            this._workerControl.Name = "_workerControl";
            this._workerControl.Size = new System.Drawing.Size(300, 40);
            this._workerControl.TabIndex = 4;
            this._workerControl.WorkerCountChanged += new System.Action<int>(this.OnWorkerCountChanged);
            // 
            // _presetLabel
            // 
            this._presetLabel.AutoSize = true;
            this._presetLabel.Location = new System.Drawing.Point(10, 35);
            this._presetLabel.Name = "_presetLabel";
            this._presetLabel.Size = new System.Drawing.Size(58, 20);
            this._presetLabel.TabIndex = 5;
            this._presetLabel.Text = "Preset:";
            // 
            // _presetComboBox
            // 
            this._presetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._presetComboBox.Location = new System.Drawing.Point(74, 32);
            this._presetComboBox.Name = "_presetComboBox";
            this._presetComboBox.Size = new System.Drawing.Size(200, 28);
            this._presetComboBox.TabIndex = 6;
            this._presetComboBox.SelectedIndexChanged += new System.EventHandler(this.OnPresetComboBoxSelectedIndexChanged);
            // 
            // _savePresetButton
            // 
            this._savePresetButton.Location = new System.Drawing.Point(280, 30);
            this._savePresetButton.Name = "_savePresetButton";
            this._savePresetButton.Size = new System.Drawing.Size(100, 30);
            this._savePresetButton.TabIndex = 7;
            this._savePresetButton.Text = "Save Preset";
            this._savePresetButton.UseVisualStyleBackColor = true;
            this._savePresetButton.Click += new System.EventHandler(this.OnSavePresetButtonClick);
            // 
            // _deletePresetButton
            // 
            this._deletePresetButton.Location = new System.Drawing.Point(385, 30);
            this._deletePresetButton.Name = "_deletePresetButton";
            this._deletePresetButton.Size = new System.Drawing.Size(100, 30);
            this._deletePresetButton.TabIndex = 8;
            this._deletePresetButton.Text = "Delete Preset";
            this._deletePresetButton.UseVisualStyleBackColor = true;
            this._deletePresetButton.Click += new System.EventHandler(this.OnDeletePresetButtonClick);
            // 
            // _filterPanel
            // 
            this._filterPanel.Controls.Add(this._includeExtensionsBox);
            this._filterPanel.Controls.Add(this._excludeExtensionsBox);
            this._filterPanel.Controls.Add(this._includePathsBox);
            this._filterPanel.Controls.Add(this._excludePathsBox);
            this._filterPanel.Controls.Add(this._maxDepthBox);
            this._filterPanel.Controls.Add(this._minSizeBox);
            this._filterPanel.Controls.Add(this._maxSizeBox);
            this._filterPanel.Controls.Add(this._skipHiddenCheck);
            this._filterPanel.Controls.Add(this._skipSystemCheck);
            this._filterPanel.Controls.Add(this._detectDuplicatesCheck);
            this._filterPanel.Controls.Add(this._duplicateMinSizeBox);
            this._filterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterPanel.Location = new System.Drawing.Point(3, 63);
            this._filterPanel.Name = "_filterPanel";
            this._filterPanel.Size = new System.Drawing.Size(651, 231);
            this._filterPanel.TabIndex = 1;
            // 
            // _includeExtensionsBox
            // 
            this._includeExtensionsBox.Location = new System.Drawing.Point(150, 10);
            this._includeExtensionsBox.Name = "_includeExtensionsBox";
            this._includeExtensionsBox.Size = new System.Drawing.Size(490, 27);
            this._includeExtensionsBox.TabIndex = 0;
            this._includeExtensionsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);
            // 
            // _excludeExtensionsBox
            // 
            this._excludeExtensionsBox.Location = new System.Drawing.Point(150, 45);
            this._excludeExtensionsBox.Name = "_excludeExtensionsBox";
            this._excludeExtensionsBox.Size = new System.Drawing.Size(490, 27);
            this._excludeExtensionsBox.TabIndex = 1;
            this._excludeExtensionsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);
            // 
            // _includePathsBox
            // 
            this._includePathsBox.Location = new System.Drawing.Point(150, 80);
            this._includePathsBox.Name = "_includePathsBox";
            this._includePathsBox.Size = new System.Drawing.Size(490, 27);
            this._includePathsBox.TabIndex = 2;
            this._includePathsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);
            // 
            // _excludePathsBox
            // 
            this._excludePathsBox.Location = new System.Drawing.Point(150, 115);
            this._excludePathsBox.Name = "_excludePathsBox";
            this._excludePathsBox.Size = new System.Drawing.Size(490, 27);
            this._excludePathsBox.TabIndex = 3;
            this._excludePathsBox.TextChanged += new System.EventHandler(this.OnFilterBoxChanged);
            // 
            // _maxDepthBox
            // 
            this._maxDepthBox.Location = new System.Drawing.Point(150, 150);
            this._maxDepthBox.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this._maxDepthBox.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            this._maxDepthBox.Name = "_maxDepthBox";
            this._maxDepthBox.Size = new System.Drawing.Size(120, 27);
            this._maxDepthBox.TabIndex = 4;
            this._maxDepthBox.Value = new decimal(new int[] { -1, 0, 0, int.MinValue });
            // 
            // _minSizeBox
            // 
            this._minSizeBox.Location = new System.Drawing.Point(300, 150);
            this._minSizeBox.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            this._minSizeBox.Name = "_minSizeBox";
            this._minSizeBox.Size = new System.Drawing.Size(150, 27);
            this._minSizeBox.TabIndex = 5;
            // 
            // _maxSizeBox
            // 
            this._maxSizeBox.Location = new System.Drawing.Point(470, 150);
            this._maxSizeBox.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            this._maxSizeBox.Name = "_maxSizeBox";
            this._maxSizeBox.Size = new System.Drawing.Size(150, 27);
            this._maxSizeBox.TabIndex = 6;
            // 
            // _skipHiddenCheck
            // 
            this._skipHiddenCheck.AutoSize = true;
            this._skipHiddenCheck.Checked = true;
            this._skipHiddenCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this._skipHiddenCheck.Location = new System.Drawing.Point(10, 190);
            this._skipHiddenCheck.Name = "_skipHiddenCheck";
            this._skipHiddenCheck.Size = new System.Drawing.Size(170, 24);
            this._skipHiddenCheck.TabIndex = 7;
            this._skipHiddenCheck.Text = "Skip hidden files/dirs";
            this._skipHiddenCheck.UseVisualStyleBackColor = true;
            // 
            // _skipSystemCheck
            // 
            this._skipSystemCheck.AutoSize = true;
            this._skipSystemCheck.Location = new System.Drawing.Point(190, 190);
            this._skipSystemCheck.Name = "_skipSystemCheck";
            this._skipSystemCheck.Size = new System.Drawing.Size(170, 24);
            this._skipSystemCheck.TabIndex = 8;
            this._skipSystemCheck.Text = "Skip system files/dirs";
            this._skipSystemCheck.UseVisualStyleBackColor = true;
            // 
            // _detectDuplicatesCheck
            // 
            this._detectDuplicatesCheck.AutoSize = true;
            this._detectDuplicatesCheck.Location = new System.Drawing.Point(370, 190);
            this._detectDuplicatesCheck.Name = "_detectDuplicatesCheck";
            this._detectDuplicatesCheck.Size = new System.Drawing.Size(210, 24);
            this._detectDuplicatesCheck.TabIndex = 9;
            this._detectDuplicatesCheck.Text = "Detect duplicates (SHA256)";
            this._detectDuplicatesCheck.UseVisualStyleBackColor = true;
            // 
            // _duplicateMinSizeBox
            // 
            this._duplicateMinSizeBox.Location = new System.Drawing.Point(150, 210);
            this._duplicateMinSizeBox.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            this._duplicateMinSizeBox.Minimum = new decimal(new int[] { 1048576, 0, 0, 0 });
            this._duplicateMinSizeBox.Name = "_duplicateMinSizeBox";
            this._duplicateMinSizeBox.Size = new System.Drawing.Size(150, 27);
            this._duplicateMinSizeBox.TabIndex = 10;
            this._duplicateMinSizeBox.Value = new decimal(new int[] { 1048576, 0, 0, 0 });
            // 
            // _progressPanel
            // 
            this._progressPanel.Controls.Add(this._progressBar);
            this._progressPanel.Controls.Add(this._elapsedLabel);
            this._progressPanel.Controls.Add(this._etaLabel);
            this._progressPanel.Controls.Add(this._fileCountLabel);
            this._progressPanel.Controls.Add(this._errorCountLabel);
            this._progressPanel.Controls.Add(this._statusLabel);
            this._progressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._progressPanel.Location = new System.Drawing.Point(3, 300);
            this._progressPanel.Name = "_progressPanel";
            this._progressPanel.Size = new System.Drawing.Size(651, 74);
            this._progressPanel.TabIndex = 2;
            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(10, 10);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(630, 25);
            this._progressBar.TabIndex = 0;
            // 
            // _elapsedLabel
            // 
            this._elapsedLabel.AutoSize = true;
            this._elapsedLabel.Location = new System.Drawing.Point(10, 45);
            this._elapsedLabel.Name = "_elapsedLabel";
            this._elapsedLabel.Size = new System.Drawing.Size(0, 20);
            this._elapsedLabel.TabIndex = 1;
            // 
            // _etaLabel
            // 
            this._etaLabel.AutoSize = true;
            this._etaLabel.Location = new System.Drawing.Point(200, 45);
            this._etaLabel.Name = "_etaLabel";
            this._etaLabel.Size = new System.Drawing.Size(0, 20);
            this._etaLabel.TabIndex = 2;
            // 
            // _fileCountLabel
            // 
            this._fileCountLabel.AutoSize = true;
            this._fileCountLabel.Location = new System.Drawing.Point(400, 45);
            this._fileCountLabel.Name = "_fileCountLabel";
            this._fileCountLabel.Size = new System.Drawing.Size(0, 20);
            this._fileCountLabel.TabIndex = 3;
            // 
            // _errorCountLabel
            // 
            this._errorCountLabel.AutoSize = true;
            this._errorCountLabel.Location = new System.Drawing.Point(550, 45);
            this._errorCountLabel.Name = "_errorCountLabel";
            this._errorCountLabel.Size = new System.Drawing.Size(0, 20);
            this._errorCountLabel.TabIndex = 4;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Location = new System.Drawing.Point(10, 65);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(45, 20);
            this._statusLabel.TabIndex = 5;
            this._statusLabel.Text = "Ready.";
            // 
            // _resultsPanel
            // 
            this._resultsPanel.Controls.Add(this._searchBox);
            this._resultsPanel.Controls.Add(this._searchLabel);
            this._resultsPanel.Controls.Add(this._minSizeFilterBox);
            this._resultsPanel.Controls.Add(this._minSizeFilterLabel);
            this._resultsPanel.Controls.Add(this._resultsDataGridView);
            this._resultsPanel.Controls.Add(this._exportButton);
            this._resultsPanel.Controls.Add(this._recycleBinButton);
            this._resultsPanel.Controls.Add(this._hardDeleteButton);
            this._resultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsPanel.Location = new System.Drawing.Point(3, 380);
            this._resultsPanel.Name = "_resultsPanel";
            this._resultsPanel.Size = new System.Drawing.Size(651, 417);
            this._resultsPanel.TabIndex = 3;
            // 
            // _searchBox
            // 
            this._searchBox.Location = new System.Drawing.Point(100, 10);
            this._searchBox.Name = "_searchBox";
            this._searchBox.Size = new System.Drawing.Size(300, 27);
            this._searchBox.TabIndex = 0;
            this._searchBox.TextChanged += new System.EventHandler(this.OnSearchBoxTextChanged);
            // 
            // _searchLabel
            // 
            this._searchLabel.AutoSize = true;
            this._searchLabel.Location = new System.Drawing.Point(10, 13);
            this._searchLabel.Name = "_searchLabel";
            this._searchLabel.Size = new System.Drawing.Size(58, 20);
            this._searchLabel.TabIndex = 1;
            this._searchLabel.Text = "Search:";
            // 
            // _minSizeFilterBox
            // 
            this._minSizeFilterBox.Location = new System.Drawing.Point(430, 8);
            this._minSizeFilterBox.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            this._minSizeFilterBox.Name = "_minSizeFilterBox";
            this._minSizeFilterBox.Size = new System.Drawing.Size(150, 27);
            this._minSizeFilterBox.TabIndex = 2;
            this._minSizeFilterBox.ValueChanged += new System.EventHandler(this.OnMinSizeFilterChanged);
            // 
            // _minSizeFilterLabel
            // 
            this._minSizeFilterLabel.AutoSize = true;
            this._minSizeFilterLabel.Location = new System.Drawing.Point(340, 13);
            this._minSizeFilterLabel.Name = "_minSizeFilterLabel";
            this._minSizeFilterLabel.Size = new System.Drawing.Size(80, 20);
            this._minSizeFilterLabel.TabIndex = 3;
            this._minSizeFilterLabel.Text = "Min size:";
            // 
            // _resultsDataGridView
            // 
            this._resultsDataGridView.AllowUserToAddRows = false;
            this._resultsDataGridView.AllowUserToDeleteRows = false;
            this._resultsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._resultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._resultsDataGridView.Location = new System.Drawing.Point(10, 45);
            this._resultsDataGridView.MultiSelect = true;
            this._resultsDataGridView.Name = "_resultsDataGridView";
            this._resultsDataGridView.ReadOnly = true;
            this._resultsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._resultsDataGridView.Size = new System.Drawing.Size(630, 300);
            this._resultsDataGridView.TabIndex = 4;
            // 
            // _exportButton
            // 
            this._exportButton.Location = new System.Drawing.Point(10, 355);
            this._exportButton.Name = "_exportButton";
            this._exportButton.Size = new System.Drawing.Size(120, 32);
            this._exportButton.TabIndex = 5;
            this._exportButton.Text = "Export Report...";
            this._exportButton.UseVisualStyleBackColor = true;
            this._exportButton.Click += new System.EventHandler(this.OnExportButtonClick);
            // 
            // _recycleBinButton
            // 
            this._recycleBinButton.Location = new System.Drawing.Point(140, 355);
            this._recycleBinButton.Name = "_recycleBinButton";
            this._recycleBinButton.Size = new System.Drawing.Size(160, 32);
            this._recycleBinButton.TabIndex = 6;
            this._recycleBinButton.Text = "Send to Recycle Bin";
            this._recycleBinButton.UseVisualStyleBackColor = true;
            this._recycleBinButton.Click += new System.EventHandler(this.OnRecycleBinButtonClick);
            // 
            // _hardDeleteButton
            // 
            this._hardDeleteButton.Location = new System.Drawing.Point(310, 355);
            this._hardDeleteButton.Name = "_hardDeleteButton";
            this._hardDeleteButton.Size = new System.Drawing.Size(160, 32);
            this._hardDeleteButton.TabIndex = 7;
            this._hardDeleteButton.Text = "Hard Delete (final)";
            this._hardDeleteButton.UseVisualStyleBackColor = true;
            this._hardDeleteButton.Click += new System.EventHandler(this.OnHardDeleteButtonClick);
            // 
            // _systemInfoPanel
            // 
            this._systemInfoPanel.Controls.Add(this._systemInfoGrid);
            this._systemInfoPanel.Controls.Add(this._refreshSystemInfoButton);
            this._systemInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._systemInfoPanel.Location = new System.Drawing.Point(660, 63);
            this._systemInfoPanel.Name = "_systemInfoPanel";
            this._systemInfoPanel.Size = new System.Drawing.Size(437, 231);
            this._systemInfoPanel.TabIndex = 4;
            // 
            // _systemInfoGrid
            // 
            this._systemInfoGrid.AllowUserToAddRows = false;
            this._systemInfoGrid.AllowUserToDeleteRows = false;
            this._systemInfoGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._systemInfoGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._systemInfoGrid.Location = new System.Drawing.Point(10, 40);
            this._systemInfoGrid.Name = "_systemInfoGrid";
            this._systemInfoGrid.ReadOnly = true;
            this._systemInfoGrid.Size = new System.Drawing.Size(420, 180);
            this._systemInfoGrid.TabIndex = 0;
            // 
            // _refreshSystemInfoButton
            // 
            this._refreshSystemInfoButton.Location = new System.Drawing.Point(10, 8);
            this._refreshSystemInfoButton.Name = "_refreshSystemInfoButton";
            this._refreshSystemInfoButton.Size = new System.Drawing.Size(150, 30);
            this._refreshSystemInfoButton.TabIndex = 1;
            this._refreshSystemInfoButton.Text = "Refresh System Info";
            this._refreshSystemInfoButton.UseVisualStyleBackColor = true;
            this._refreshSystemInfoButton.Click += new System.EventHandler(this.OnRefreshSystemInfoClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 800);
            this.Controls.Add(this._mainTable);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Win11DeTrasher — Disk Space Analyzer & Cleaner";
            this._mainTable.ResumeLayout(false);
            this._mainTable.PerformLayout();
            this._topPanel.ResumeLayout(false);
            this._topPanel.PerformLayout();
            this._filterPanel.ResumeLayout(false);
            this._filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._maxDepthBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._minSizeBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._maxSizeBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._duplicateMinSizeBox).EndInit();
            this._progressPanel.ResumeLayout(false);
            this._progressPanel.PerformLayout();
            this._resultsPanel.ResumeLayout(false);
            this._resultsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._minSizeFilterBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._resultsDataGridView).EndInit();
            this._systemInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._systemInfoGrid).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
        #nullable restore

        private System.Windows.Forms.TableLayoutPanel _mainTable = null!;
        private System.Windows.Forms.Panel _topPanel = null!;
        private System.Windows.Forms.Label _driveLabel = null!;
        private System.Windows.Forms.ComboBox _driveComboBox = null!;
        private System.Windows.Forms.Button _scanButton = null!;
        private System.Windows.Forms.Button _cancelButton = null!;
        private Controls.WorkerControl _workerControl = null!;
        private System.Windows.Forms.Label _presetLabel = null!;
        private System.Windows.Forms.ComboBox _presetComboBox = null!;
        private System.Windows.Forms.Button _savePresetButton = null!;
        private System.Windows.Forms.Button _deletePresetButton = null!;
        private System.Windows.Forms.Panel _filterPanel = null!;
        private System.Windows.Forms.TextBox _includeExtensionsBox = null!;
        private System.Windows.Forms.TextBox _excludeExtensionsBox = null!;
        private System.Windows.Forms.TextBox _includePathsBox = null!;
        private System.Windows.Forms.TextBox _excludePathsBox = null!;
        private System.Windows.Forms.NumericUpDown _maxDepthBox = null!;
        private System.Windows.Forms.NumericUpDown _minSizeBox = null!;
        private System.Windows.Forms.NumericUpDown _maxSizeBox = null!;
        private System.Windows.Forms.CheckBox _skipHiddenCheck = null!;
        private System.Windows.Forms.CheckBox _skipSystemCheck = null!;
        private System.Windows.Forms.CheckBox _detectDuplicatesCheck = null!;
        private System.Windows.Forms.NumericUpDown _duplicateMinSizeBox = null!;
        private System.Windows.Forms.Panel _progressPanel = null!;
        private System.Windows.Forms.ProgressBar _progressBar = null!;
        private System.Windows.Forms.Label _elapsedLabel = null!;
        private System.Windows.Forms.Label _etaLabel = null!;
        private System.Windows.Forms.Label _fileCountLabel = null!;
        private System.Windows.Forms.Label _errorCountLabel = null!;
        private System.Windows.Forms.Label _statusLabel = null!;
        private System.Windows.Forms.Panel _resultsPanel = null!;
        private System.Windows.Forms.TextBox _searchBox = null!;
        private System.Windows.Forms.Label _searchLabel = null!;
        private System.Windows.Forms.NumericUpDown _minSizeFilterBox = null!;
        private System.Windows.Forms.Label _minSizeFilterLabel = null!;
        private Controls.SortableDataGridView _resultsDataGridView = null!;
        private System.Windows.Forms.Button _exportButton = null!;
        private System.Windows.Forms.Button _recycleBinButton = null!;
        private System.Windows.Forms.Button _hardDeleteButton = null!;
        private System.Windows.Forms.Panel _systemInfoPanel = null!;
        private System.Windows.Forms.DataGridView _systemInfoGrid = null!;
        private System.Windows.Forms.Button _refreshSystemInfoButton = null!;
    }
}
