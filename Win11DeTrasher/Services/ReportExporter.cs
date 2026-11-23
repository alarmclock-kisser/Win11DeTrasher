using System.Text;
using System.Text.Json;
using Win11DeTrasher.Models;
using Win11DeTrasher.Utils;

namespace Win11DeTrasher.Services;

/// <summary>
/// Default implementation of <see cref="IReportExporter"/>.
/// Exports scan results to CSV, JSON, HTML, and TXT formats.
/// </summary>
public class ReportExporter : IReportExporter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <inheritdoc/>
    public bool ExportCsv(IList<ScanResult> results, string filePath)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("Full Path,Name,Extension,Size Bytes,Size Display,Depth,Is Directory,Last Write Time,File Count,Directory Count");

            foreach (var r in results)
            {
                sb.AppendLine(string.Join(",",
                    CsvEscape(r.FullPath),
                    CsvEscape(r.Name),
                    CsvEscape(r.Extension),
                    r.SizeBytes.ToString(),
                    CsvEscape(r.SizeDisplay),
                    r.Depth.ToString(),
                    r.IsDirectory.ToString(),
                    r.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    r.FileCount.ToString(),
                    r.DirectoryCount.ToString()));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool ExportJson(IList<ScanResult> results, string filePath)
    {
        try
        {
            var exportData = results.Select(r => new
            {
                r.FullPath,
                r.Name,
                r.Extension,
                r.SizeBytes,
                r.SizeDisplay,
                r.Depth,
                r.IsDirectory,
                LastWriteTime = r.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                r.FileCount,
                r.DirectoryCount
            });

            var json = JsonSerializer.Serialize(exportData, JsonOptions);
            File.WriteAllText(filePath, json, Encoding.UTF8);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool ExportHtml(IList<ScanResult> results, string filePath)
    {
        try
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\">");
            sb.AppendLine("<title>Win11DeTrasher Scan Report</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; }");
            sb.AppendLine("h1 { color: #2c5f8a; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("th, td { padding: 8px 12px; border: 1px solid #ddd; text-align: left; }");
            sb.AppendLine("th { background-color: #2c5f8a; color: white; }");
            sb.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }");
            sb.AppendLine(".dir { font-weight: bold; color: #2c5f8a; }");
            sb.AppendLine(".summary { margin-bottom: 20px; padding: 10px; background-color: #e8f0fe; border-radius: 4px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Win11DeTrasher Scan Report</h1>");

            var totalBytes = results.Where(r => !r.IsDirectory).Sum(r => r.SizeBytes);
            var fileCount = results.Count(r => !r.IsDirectory);
            var dirCount = results.Count(r => r.IsDirectory);

            sb.AppendLine("<div class=\"summary\">");
            sb.AppendLine($"<strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}<br>");
            sb.AppendLine($"<strong>Total Files:</strong> {fileCount:N0}<br>");
            sb.AppendLine($"<strong>Total Directories:</strong> {dirCount:N0}<br>");
            sb.AppendLine($"<strong>Total Size:</strong> {SizeFormatter.Format(totalBytes)}");
            sb.AppendLine("</div>");

            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Path</th><th>Name</th><th>Ext</th><th>Size</th><th>Depth</th><th>Type</th><th>Last Write</th></tr>");

            foreach (var r in results.OrderByDescending(r => r.SizeBytes))
            {
                var typeClass = r.IsDirectory ? "dir" : "";
                var typeText = r.IsDirectory ? "DIR" : "FILE";
                sb.AppendLine($"<tr class=\"{typeClass}\">");
                sb.AppendLine($"<td>{HtmlEscape(r.FullPath)}</td>");
                sb.AppendLine($"<td>{HtmlEscape(r.Name)}</td>");
                sb.AppendLine($"<td>{HtmlEscape(r.Extension)}</td>");
                sb.AppendLine($"<td>{r.SizeDisplay}</td>");
                sb.AppendLine($"<td>{r.Depth}</td>");
                sb.AppendLine($"<td>{typeText}</td>");
                sb.AppendLine($"<td>{r.LastWriteTime:yyyy-MM-dd HH:mm:ss}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool ExportTxt(IList<ScanResult> results, string filePath)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("Win11DeTrasher Scan Report");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine(new string('=', 80));
            sb.AppendLine();

            var totalBytes = results.Where(r => !r.IsDirectory).Sum(r => r.SizeBytes);
            var fileCount = results.Count(r => !r.IsDirectory);
            var dirCount = results.Count(r => r.IsDirectory);

            sb.AppendLine($"Total Files:     {fileCount:N0}");
            sb.AppendLine($"Total Directories: {dirCount:N0}");
            sb.AppendLine($"Total Size:      {SizeFormatter.Format(totalBytes)}");
            sb.AppendLine();
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            foreach (var r in results.OrderByDescending(r => r.SizeBytes))
            {
                var type = r.IsDirectory ? "[DIR ]" : "[FILE]";
                var sizeText = r.SizeDisplay.PadLeft(12);
                sb.AppendLine($"{type} {sizeText}  {r.FullPath}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string CsvEscape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    private static string HtmlEscape(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");
    }
}
