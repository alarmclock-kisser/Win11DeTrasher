using System.ComponentModel;
using System.Windows.Forms;
using Win11DeTrasher.Models;

namespace Win11DeTrasher.Controls;

/// <summary>
/// A <see cref="DataGridView"/> subclass that provides column-header click sorting
/// with three-way cycling: ascending → descending → default (original) order.
/// The sort is applied in-memory to the bound data source and the grid is
/// re-bound after each sort change.
/// </summary>
public class SortableDataGridView : DataGridView
{
    private Dictionary<string, int> _sortState = new(StringComparer.OrdinalIgnoreCase);
    private string? _lastSortedColumn;

    /// <summary>
    /// Event raised when the sort state changes.
    /// </summary>
    public event Action<string, int>? SortStateChanged;

    /// <summary>
    /// Gets the current sort direction for the last sorted column.
    /// 1 = ascending, -1 = descending, 0 = default (original) order.
    /// </summary>
    public int CurrentSortDirection
    {
        get
        {
            if (this._lastSortedColumn == null)
            {
                return 0;
            }

            return this._sortState.TryGetValue(this._lastSortedColumn, out var dir) ? dir : 0;
        }
    }

    public SortableDataGridView()
    {
        this.AllowUserToOrderColumns = false;
        this.ColumnHeaderMouseClick += this.OnColumnHeaderMouseClick;
    }

    private void OnColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex < 0 || e.ColumnIndex >= this.Columns.Count)
        {
            return;
        }

        var column = this.Columns[e.ColumnIndex];
        var columnName = column.Name;

        if (string.IsNullOrEmpty(columnName))
        {
            return;
        }

        this.ApplySort(columnName);
    }

    /// <summary>
    /// Applies a sort to the specified column, cycling through
    /// ascending → descending → default order.
    /// </summary>
    public void ApplySort(string columnName)
    {
        int direction;
        if (this._lastSortedColumn != columnName)
        {
            direction = 1;
        }
        else
        {
            this._sortState.TryGetValue(columnName, out var currentDir);
            direction = currentDir switch
            {
                1 => -1,
                -1 => 0,
                _ => 1
            };
        }

        this._sortState[columnName] = direction;
        this._lastSortedColumn = columnName;

        this.ApplySortToDataSource(columnName, direction);
        this.SortStateChanged?.Invoke(columnName, direction);
    }

    /// <summary>
    /// Resets the sort to the default (original) order.
    /// </summary>
    public void ResetSort()
    {
        this._sortState.Clear();
        this._lastSortedColumn = null;
        this.ApplySortToDataSource(null, 0);
        this.SortStateChanged?.Invoke("default", 0);
    }

    private void ApplySortToDataSource(string? columnName, int direction)
    {
        var list = this.DataSource as List<ScanResult>;
        if (list == null)
        {
            return;
        }

        if (columnName == null || direction == 0)
        {
            list.Sort((a, b) => b.SizeBytes.CompareTo(a.SizeBytes));
        }
        else
        {
            var column = this.Columns[columnName];
            if (column == null)
            {
                return;
            }

            var propName = column.DataPropertyName;
            if (string.IsNullOrEmpty(propName))
            {
                return;
            }

            var property = typeof(ScanResult).GetProperty(propName);
            if (property == null)
            {
                return;
            }

            if (direction == 1)
            {
                list.Sort((a, b) => CompareByProperty(a, b, property));
            }
            else
            {
                list.Sort((a, b) => CompareByProperty(b, a, property));
            }
        }

        // Re-bind to trigger grid refresh
        var currentSource = this.DataSource;
        this.DataSource = null;
        this.DataSource = currentSource;
    }

    private static int CompareByProperty(ScanResult a, ScanResult b, System.Reflection.PropertyInfo property)
    {
        var va = property.GetValue(a) as IComparable;
        var vb = property.GetValue(b) as IComparable;

        if (va == null && vb == null) return 0;
        if (va == null) return -1;
        if (vb == null) return 1;
        return va.CompareTo(vb);
    }
}
