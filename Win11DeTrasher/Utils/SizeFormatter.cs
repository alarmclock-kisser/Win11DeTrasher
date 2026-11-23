using System.Globalization;

namespace Win11DeTrasher.Utils;

/// <summary>
/// Static helpers for formatting byte counts into human-readable strings.
/// All formatting and parsing uses <see cref="CultureInfo.InvariantCulture"/>
/// to ensure consistent output regardless of the system's locale.
/// </summary>
public static class SizeFormatter
{
    private static readonly string[] Suffixes = ["B", "KB", "MB", "GB", "TB", "PB"];
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    /// <summary>
    /// Formats a byte count into a human-readable string (e.g. "1.23 GB").
    /// Uses invariant culture so the decimal separator is always '.'.
    /// </summary>
    public static string Format(long bytes)
    {
        if (bytes < 0)
        {
            return "-" + Format(-bytes);
        }

        double value = bytes;
        int index = 0;

        while (value >= 1024 && index < Suffixes.Length - 1)
        {
            value /= 1024;
            index++;
        }

        return index == 0
            ? value.ToString("0", Invariant) + " " + Suffixes[index]
            : value.ToString("0.##", Invariant) + " " + Suffixes[index];
    }

    /// <summary>
    /// Parses a human-readable size string (e.g. "1.5 GB") into a byte count.
    /// Accepts both '.' and ',' as decimal separators for user convenience.
    /// Uses invariant culture for the actual parsing.
    /// </summary>
    public static bool TryParse(string text, out long bytes)
    {
        bytes = 0;

        var trimmed = text.Trim().ToUpperInvariant();
        if (trimmed.Length == 0)
        {
            return false;
        }

        int splitIndex = 0;
        for (int i = 0; i < trimmed.Length; i++)
        {
            if (!char.IsDigit(trimmed[i]) && trimmed[i] != '.' && trimmed[i] != ',')
            {
                splitIndex = i;
                break;
            }

            splitIndex = i + 1;
        }

        if (splitIndex == 0 || splitIndex == trimmed.Length)
        {
            var numPart = trimmed.Replace(",", ".");
            return double.TryParse(numPart, NumberStyles.Float, Invariant, out double d) && (bytes = (long)d) >= 0;
        }

        var numberPart = trimmed[..splitIndex].Replace(",", ".");
        var unitPart = trimmed[splitIndex..].Trim();

        if (!double.TryParse(numberPart, NumberStyles.Float, Invariant, out double value))
        {
            return false;
        }

        double multiplier = unitPart switch
        {
            "B" => 1,
            "KB" => 1024,
            "MB" => 1024 * 1024,
            "GB" => 1024L * 1024 * 1024,
            "TB" => 1024L * 1024 * 1024 * 1024,
            "PB" => 1024L * 1024 * 1024 * 1024 * 1024,
            _ => 0
        };

        if (multiplier == 0)
        {
            return false;
        }

        bytes = (long)(value * multiplier);
        return true;
    }
}
