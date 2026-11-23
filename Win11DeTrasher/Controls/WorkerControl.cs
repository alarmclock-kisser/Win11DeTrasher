using System.ComponentModel;
using System.Windows.Forms;

namespace Win11DeTrasher.Controls;

/// <summary>
/// A control that allows the user to adjust the number of scanner worker
/// threads on-the-fly during a running scan.
/// </summary>
public class WorkerControl : UserControl
{
    private readonly NumericUpDown _numericUpDown = new();
    private readonly TrackBar _trackBar = new();
    private readonly Label _label = new();
    private readonly Label _valueLabel = new();
    private int _maxWorkers;
    private int _workerCount;

    /// <summary>
    /// Event raised when the worker count changes.
    /// </summary>
    public event Action<int>? WorkerCountChanged;

    /// <summary>
    /// Gets or sets the current worker count.
    /// </summary>
    [DefaultValue(4)]
    public int WorkerCount
    {
        get => this._workerCount;
        set
        {
            var clamped = Math.Clamp(value, 1, this._maxWorkers);
            this._workerCount = clamped;
            this._numericUpDown.Value = clamped;
            this._trackBar.Value = clamped;
            this.UpdateValueLabel();
        }
    }

    /// <summary>
    /// Gets the maximum number of workers.
    /// </summary>
    public int MaxWorkers => this._maxWorkers;

    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    [DefaultValue(true)]
    public bool IsEditable
    {
        get => this._numericUpDown.Enabled;
        set
        {
            this._numericUpDown.Enabled = value;
            this._trackBar.Enabled = value;
        }
    }

    public WorkerControl()
    {
        this._maxWorkers = Math.Max(1, Environment.ProcessorCount);
        this._workerCount = Math.Min(4, this._maxWorkers);

        this._label.Text = "Workers:";
        this._label.AutoSize = true;

        this._valueLabel.Text = string.Empty;
        this._valueLabel.AutoSize = true;
        this._valueLabel.ForeColor = System.Drawing.Color.Gray;

        this._numericUpDown.Minimum = 1;
        this._numericUpDown.Maximum = this._maxWorkers;
        this._numericUpDown.Value = this._workerCount;
        this._numericUpDown.Width = 60;
        this._numericUpDown.ValueChanged += this.OnValueChange;

        this._trackBar.Minimum = 1;
        this._trackBar.Maximum = this._maxWorkers;
        this._trackBar.Value = this._workerCount;
        this._trackBar.Width = 150;
        this._trackBar.TickFrequency = 1;
        this._trackBar.Scroll += this.OnTrackBarScroll;

        this.Controls.Add(this._label);
        this.Controls.Add(this._numericUpDown);
        this.Controls.Add(this._trackBar);
        this.Controls.Add(this._valueLabel);

        this.Height = 40;
        this.UpdateValueLabel();
    }

    private void OnValueChange(object? sender, EventArgs e)
    {
        var value = (int)this._numericUpDown.Value;
        this._workerCount = value;
        this._trackBar.Value = value;
        this.UpdateValueLabel();
        this.WorkerCountChanged?.Invoke(value);
    }

    private void OnTrackBarScroll(object? sender, EventArgs e)
    {
        var value = this._trackBar.Value;
        this._workerCount = value;
        this._numericUpDown.Value = value;
        this.UpdateValueLabel();
        this.WorkerCountChanged?.Invoke(value);
    }

    private void UpdateValueLabel()
    {
        this._valueLabel.Text = $"({this._workerCount}/{this._maxWorkers} cores)";
    }
}
