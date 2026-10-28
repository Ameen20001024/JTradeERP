using System.Globalization;
using System.Text;

namespace JTradeERP.Application.Common.Reports;

public static class CsvReportFormatter
{
    public static byte[] Create<T>(IReadOnlyCollection<string> headers, IEnumerable<T> rows, Func<T, IReadOnlyList<object?>> values)
    {
        var builder = new StringBuilder();
        AppendRow(builder, headers.Cast<object?>());
        foreach (var row in rows) AppendRow(builder, values(row));
        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(builder.ToString());
    }

    private static void AppendRow(StringBuilder builder, IEnumerable<object?> values)
    {
        var first = true;
        foreach (var value in values)
        {
            if (!first) builder.Append(',');
            first = false;
            var text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            if (value is string && text.TrimStart() is var candidate && candidate.Length > 0 && candidate[0] is '=' or '+' or '-' or '@' or '\t' or '\r')
                text = "'" + text;
            builder.Append('"').Append(text.Replace("\"", "\"\"", StringComparison.Ordinal)).Append('"');
        }
        builder.Append("\r\n");
    }
}
