using System.Text;
using System.Text.RegularExpressions;

namespace Orion.Network.Core.Parsers;

public partial class NewLineMessageParser
{
    private static readonly Regex _splitRegex = NewLineRegex();

    [GeneratedRegex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex NewLineRegex();


    public static string[] ParseMessages(ReadOnlyMemory<byte> buffer)
    {
        // readonly memory to byte array
        var byteArray = new byte[buffer.Length];
        buffer.Span.CopyTo(byteArray);

        var content = Encoding.UTF8.GetString(byteArray);
        var messages = _splitRegex.Split(content);


        return Array.FindAll(messages, msg => !string.IsNullOrWhiteSpace(msg));
    }

    public static List<string> FastParseMessages(ReadOnlyMemory<byte> buffer)
    {
        var messages = new List<string>();
        var span = buffer.Span;

        int start = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == (byte)'\n' || span[i] == (byte)'\r')
            {
                if (i > start)
                {
                    var lineSpan = span.Slice(start, i - start);
                    messages.Add(Encoding.UTF8.GetString(lineSpan));
                }


                if (span[i] == (byte)'\r' && i + 1 < span.Length && span[i + 1] == (byte)'\n')
                {
                    i++;
                }

                start = i + 1;
            }
        }

        if (start < span.Length)
        {
            var lineSpan = span[start..];
            messages.Add(Encoding.UTF8.GetString(lineSpan));
        }

        return messages;
    }
}
