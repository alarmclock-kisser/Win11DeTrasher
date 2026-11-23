using System.Windows.Forms;
using Win11DeTrasher.Models;
using Win11DeTrasher.Utils;

namespace Win11DeTrasher.UI;

/// <summary>
/// A dialog for configuring scan filters.
/// </summary>
public class ScanFilterForm : Form
{
    private readonly TextBox _includeExtensionsBox = new();
    private readonly TextBox _excludeExtensionsBox = new();
    private readonly TextBox _includePathsBox = new();
    private readonly TextBox _excludePathsBox = new();
    private readonly NumericUpDown _maxDepthBox = new();
    private readonly NumericUpDown _minSizeBox = new();
    private readonly NumericUpDown _maxSizeBox = new();
    private readonly CheckBox _skipHiddenCheck = new();
    private readonly CheckBox _skipSystemCheck = new();
    private readonly CheckBox _detectDuplicatesCheck = new();
    private readonly NumericUpDown _duplicateMinSizeBox = new();
    private readonly Button _okButton = new();
    private readonly Button _cancelButton = new();

    /// <summary>
    /// Gets the configured filter options when the dialog is closed with OK.
    /// </summary>
    public ScanFilterOptions? FilterOptions { get; private set; }

    public ScanFilterForm(ScanFilterOptions? existingOptions = null)
    {
        existingOptions ??= new ScanFilterOptions();

        this.Text = "Scan Filter Options";
        this.Size = new System.Drawing.Size(520, 620);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;

        this.BuildLayout(existingOptions);

        this._okButton.Click += this.OnOkClick;
        this._cancelButton.Click += this.OnCancelClick;
    }

    private void BuildLayout(ScanFilterOptions options)
    {
        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 13,
            Padding = new Padding(10)
        };

        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200f));
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        int row = 0;

        // --- Extensions section ---
        mainPanel.Controls.Add(MakeHeader("File Extensions", row));
        mainPanel.SetColumnSpan(mainPanel.Controls[mainPanel.Controls.Count - 1], 2);
        row++;

        var lbl1 = MakeLabel("Include (comma-separated):");
        mainPanel.Controls.Add(lbl1, 0, row);
        mainPanel.Controls.Add(this._includeExtensionsBox, 1, row);
        this._includeExtensionsBox.Text = string.Join(", ", options.IncludeExtensions);
        row++;

        var lbl2 = MakeLabel("Exclude (comma-separated):");
        mainPanel.Controls.Add(lbl2, 0, row);
        mainPanel.Controls.Add(this._excludeExtensionsBox, 1, row);
        this._excludeExtensionsBox.Text = string.Join(", ", options.ExcludeExtensions);
        row++;

        // --- Path segments section ---
        mainPanel.Controls.Add(MakeHeader("Path Segments", row));
        mainPanel.SetColumnSpan(mainPanel.Controls[mainPanel.Controls.Count - 1], 2);
        row++;

        var lbl3 = MakeLabel("Include paths containing:");
        mainPanel.Controls.Add(lbl3, 0, row);
        mainPanel.Controls.Add(this._includePathsBox, 1, row);
        this._includePathsBox.Text = string.Join(", ", options.IncludePathSegments);
        row++;

        var lbl4 = MakeLabel("Exclude paths containing:");
        mainPanel.Controls.Add(lbl4, 0, row);
        mainPanel.Controls.Add(this._excludePathsBox, 1, row);
        this._excludePathsBox.Text = string.Join(", ", options.ExcludePathSegments);
        row++;

        // --- Depth ---
        mainPanel.Controls.Add(MakeHeader("Scan Depth & Size", row));
        mainPanel.SetColumnSpan(mainPanel.Controls[mainPanel.Controls.Count - 1], 2);
        row++;

        var lbl5 = MakeLabel("Max depth (-1 = unlimited):");
        mainPanel.Controls.Add(lbl5, 0, row);
        mainPanel.Controls.Add(this._maxDepthBox, 1, row);
        this._maxDepthBox.Minimum = -1;
        this._maxDepthBox.Maximum = 50;
        this._maxDepthBox.Value = options.MaxDepth;
        row++;

        var lbl6 = MakeLabel("Min size (bytes):");
        mainPanel.Controls.Add(lbl6, 0, row);
        mainPanel.Controls.Add(this._minSizeBox, 1, row);
        this._minSizeBox.Minimum = 0;
        this._minSizeBox.Maximum = long.MaxValue;
        this._minSizeBox.Value = options.MinFileSizeBytes;
        this._minSizeBox.DecimalPlaces = 0;
        row++;

        var lbl7 = MakeLabel("Max size (bytes):");
        mainPanel.Controls.Add(lbl7, 0, row);
        mainPanel.Controls.Add(this._maxSizeBox, 1, row);
        this._maxSizeBox.Minimum = 0;
        this._maxSizeBox.Maximum = long.MaxValue;
        this._maxSizeBox.Value = options.MaxFileSizeBytes < long.MaxValue ? options.MaxFileSizeBytes : long.MaxValue;
        this._maxSizeBox.DecimalPlaces = 0;
        row++;

        // --- Options ---
        mainPanel.Controls.Add(MakeHeader("Options", row));
        mainPanel.SetColumnSpan(mainPanel.Controls[mainPanel.Controls.Count - 1], 2);
        row++;

        this._skipHiddenCheck.Text = "Skip hidden files/directories";
        this._skipHiddenCheck.Checked = options.SkipHidden;
        mainPanel.Controls.Add(this._skipHiddenCheck, 0, row);
        mainPanel.SetColumnSpan(this._skipHiddenCheck, 2);
        row++;

        this._skipSystemCheck.Text = "Skip system files/directories";
        this._skipSystemCheck.Checked = options.SkipSystem;
        mainPanel.Controls.Add(this._skipSystemCheck, 0, row);
        mainPanel.SetColumnSpan(this._skipSystemCheck, 2);
        row++;

        // --- Duplicate detection ---
        this._detectDuplicatesCheck.Text = "Detect duplicate files (SHA256)";
        this._detectDuplicatesCheck.Checked = options.DetectDuplicates;
        this._detectDuplicatesCheck.CheckedChanged += this.OnDetectDuplicatesChanged;
        mainPanel.Controls.Add(this._detectDuplicatesCheck, 0, row);
        mainPanel.SetColumnSpan(this._detectDuplicatesCheck, 2);
        row++;

        var lbl8 = MakeLabel("Min size for duplicates (bytes):");
        mainPanel.Controls.Add(lbl8, 0, row);
        mainPanel.Controls.Add(this._duplicateMinSizeBox, 1, row);
        this._duplicateMinSizeBox.Minimum = 0;
        this._duplicateMinSizeBox.Maximum = long.MaxValue;
        this._duplicateMinSizeBox.Value = options.DuplicateMinSizeBytes;
        this._duplicateMinSizeBox.DecimalPlaces = 0;
        this._duplicateMinSizeBox.Enabled = options.DetectDuplicates;
        row++;

        // --- Buttons ---
        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Bottom,
            Height = 45
        };

        this._okButton.Text = "OK";
        this._okButton.Width = 80;
        this._okButton.DialogResult = DialogResult.OK;

        this._cancelButton.Text = "Cancel";
        this._cancelButton.Width = 80;
        this._cancelButton.DialogResult = DialogResult.Cancel;

        buttonPanel.Controls.Add(this._okButton);
        buttonPanel.Controls.Add(this._cancelButton);

        this.Controls.Add(mainPanel);
        this.Controls.Add(buttonPanel);
        this.AcceptButton = this._okButton;
        this.CancelButton = this._cancelButton;
    }

    private static Label MakeHeader(string text, int row)
    {
        return new Label
        {
            Text = text,
            Font = new System.Drawing.Font(SystemFonts.DefaultFont, System.Drawing.FontStyle.Bold),
            Dock = DockStyle.Fill
        };
    }

    private static Label MakeLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };
    }

    private void OnDetectDuplicatesChanged(object? sender, EventArgs e)
    {
        this._duplicateMinSizeBox.Enabled = this._detectDuplicatesCheck.Checked;
    }

    private void OnOkClick(object? sender, EventArgs e)
    {
        this.FilterOptions = this.BuildFilterOptions();
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void OnCancelClick(object? sender, EventArgs e)
    {
        this.FilterOptions = null;
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

    private ScanFilterOptions BuildFilterOptions()
    {
        return new ScanFilterOptions
        {
            MaxDepth = (int)this._maxDepthBox.Value,
            IncludeExtensions = ParseCommaList(this._includeExtensionsBox.Text),
            ExcludeExtensions = ParseCommaList(this._excludeExtensionsBox.Text),
            IncludePathSegments = ParseCommaList(this._includePathsBox.Text),
            ExcludePathSegments = ParseCommaList(this._excludePathsBox.Text),
            MinFileSizeBytes = (long)this._minSizeBox.Value,
            MaxFileSizeBytes = (long)this._maxSizeBox.Value,
            SkipHidden = this._skipHiddenCheck.Checked,
            SkipSystem = this._skipSystemCheck.Checked,
            DetectDuplicates = this._detectDuplicatesCheck.Checked,
            DuplicateMinSizeBytes = (long)this._duplicateMinSizeBox.Value
        };
    }

    private static List<string> ParseCommaList(string text)
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
}
