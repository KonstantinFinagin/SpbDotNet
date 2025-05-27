namespace SpbDotNet.Verify.Tests.Helpers;

public static class AsciiTableFormatter
{
    public static string Format(IEnumerable<object> items)
    {
        if (items == null || !items.Any())
            return "No data.";

        var properties = items.First().GetType().GetProperties();
        var headers = properties.Select(p => p.Name).ToArray();
        var rows = items.Select(item => properties.Select(p => FormatValue(p.GetValue(item))).ToArray()).ToArray();
        var columnWidths = CalculateColumnWidths(headers, rows);
        var sb = new StringBuilder();

        AppendTable(sb, headers, rows, columnWidths);

        return sb.ToString();
    }

    private static string FormatValue(object value) =>
        value switch
        {
            DateTime dt => dt.ToString("s"),
            IDictionary<string, object> dictionary => FormatDictionary(dictionary),
            IDictionary<string, string> dictionary => FormatDictionary(dictionary),
            IDictionary<string, int> intDictionary => FormatDictionary(intDictionary),
            IDictionary<char, int> charDictionary => FormatDictionary(charDictionary),
            IList<object> list => FormatList(list),
            _ => value?.ToString() ?? "" // default case
        };

    private static string FormatDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary) =>
        string.Join(", ", dictionary.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

    private static string FormatList(IList<object> list) => string.Join(", ", list);

    private static int[] CalculateColumnWidths(string[] headers, string[][] rows)
    {
        // Calculate the max width for each column
        return headers
            .Select((header, i) => Math.Max(header.Length, rows.Select(row => row[i].Length).DefaultIfEmpty().Max()))
            .ToArray();
    }

    private static void AppendTable(StringBuilder sb, string[] headers, string[][] rows, int[] columnWidths)
    {
        // Build the table and append to the StringBuilder
        string separator = BuildSeparator(columnWidths);

        sb.AppendLine(separator);
        AppendRow(sb, headers, columnWidths);
        sb.AppendLine(separator);

        foreach (var row in rows)
            AppendRow(sb, row, columnWidths);

        sb.AppendLine(separator);
    }

    private static string BuildSeparator(int[] columnWidths)
    {
        // Build the separator line for the table
        return "+-" + string.Join("-+-", columnWidths.Select(w => new string('-', w))) + "-+";
    }

    private static void AppendRow(StringBuilder sb, string[] row, int[] columnWidths)
    {
        // Format and append a single row to the StringBuilder
        string rowStr = "| " + string.Join(" | ", row.Select((cell, i) => cell.PadRight(columnWidths[i]))) + " |";
        sb.AppendLine(rowStr);
    }
}
