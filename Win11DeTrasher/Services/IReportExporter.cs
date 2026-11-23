using Win11DeTrasher.Models;

namespace Win11DeTrasher.Services;

/// <summary>
/// Abstraction for exporting scan results to various file formats
/// (CSV, JSON, HTML, TXT).
/// </summary>
public interface IReportExporter
{
    /// <summary>
    /// Exports the given scan results to a CSV file.
    /// </summary>
    /// <param name="results">The scan results to export.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <returns>True if the export succeeded, false otherwise.</returns>
    bool ExportCsv(IList<ScanResult> results, string filePath);

    /// <summary>
    /// Exports the given scan results to a JSON file.
    /// </summary>
    /// <param name="results">The scan results to export.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <returns>True if the export succeeded, false otherwise.</returns>
    bool ExportJson(IList<ScanResult> results, string filePath);

    /// <summary>
    /// Exports the given scan results to an HTML file, formatted for browser viewing.
    /// </summary>
    /// <param name="results">The scan results to export.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <returns>True if the export succeeded, false otherwise.</returns>
    bool ExportHtml(IList<ScanResult> results, string filePath);

    /// <summary>
    /// Exports the given scan results to a plain text file.
    /// </summary>
    /// <param name="results">The scan results to export.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <returns>True if the export succeeded, false otherwise.</returns>
    bool ExportTxt(IList<ScanResult> results, string filePath);
}
