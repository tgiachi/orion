namespace Orion.Foundations.Utils;

public class PortToListParserUtils
{
    public static IEnumerable<int> ParsePorts(string portRangeString)
    {
        if (string.IsNullOrWhiteSpace(portRangeString))
        {
            yield break;
        }

        // Split by comma and process each segment
        foreach (var segment in portRangeString.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedSegment = segment.Trim();

            // Check if it's a range (contains a hyphen)
            if (trimmedSegment.Contains('-'))
            {
                var rangeParts = trimmedSegment.Split('-', StringSplitOptions.RemoveEmptyEntries);

                // Validate that we have exactly two parts for a range
                if (rangeParts.Length != 2)
                {
                    throw new FormatException($"Invalid port range format: {trimmedSegment}");
                }

                // Parse start and end of the range
                if (!int.TryParse(rangeParts[0].Trim(), out int startPort) ||
                    !int.TryParse(rangeParts[1].Trim(), out int endPort))
                {
                    throw new FormatException($"Invalid port numbers in range: {trimmedSegment}");
                }

                // Validate port range
                if (startPort > endPort)
                {
                    throw new FormatException($"Invalid port range (start > end): {trimmedSegment}");
                }

                if (startPort < 0 || endPort > 65535)
                {
                    throw new FormatException($"Port numbers must be between 0 and 65535: {trimmedSegment}");
                }

                // Yield each port in the range
                for (int port = startPort; port <= endPort; port++)
                {
                    yield return port;
                }
            }
            else
            {
                // It's a single port
                if (!int.TryParse(trimmedSegment, out int port))
                {
                    throw new FormatException($"Invalid port number: {trimmedSegment}");
                }

                if (port < 0 || port > 65535)
                {
                    throw new FormatException($"Port number must be between 0 and 65535: {port}");
                }

                yield return port;
            }
        }
    }
}
