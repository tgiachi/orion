using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands;

/// <summary>
/// Represents the IRC AWAY command which allows users to indicate they are away from their computer.
/// </summary>
public class AwayCommand : BaseIrcCommand
{
    /// <summary>
    /// Source of the AWAY command (optional, used when relayed by server)
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Message explaining the reason for being away (optional)
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Initializes a new instance of the AWAY command
    /// </summary>
    public AwayCommand() : base("AWAY")
    {
    }

    /// <summary>
    /// Parses a raw IRC message to extract AWAY command data
    /// </summary>
    /// <param name="line">Raw IRC message</param>
    public override void Parse(string line)
    {
        // Reset existing data
        Source = null;
        Message = null;

        // Check for source prefix
        if (line.StartsWith(':'))
        {
            int spaceIndex = line.IndexOf(' ');
            if (spaceIndex != -1)
            {
                Source = line.Substring(1, spaceIndex - 1);
                line = line.Substring(spaceIndex + 1).TrimStart();
            }
        }

        // Split remaining parts
        string[] parts = line.Split(new[] { ' ' }, 2);

        // First token should be "AWAY"
        if (parts.Length == 0 || parts[0].ToUpper() != "AWAY")
            return;

        // Check for message (optional)
        if (parts.Length > 1)
        {
            string messagePart = parts[1];
            if (messagePart.StartsWith(":"))
            {
                Message = messagePart.Substring(1);
            }
            else
            {
                Message = messagePart;
            }
        }
    }

    /// <summary>
    /// Converts the command to its string representation
    /// </summary>
    /// <returns>Formatted AWAY command string</returns>
    public override string Write()
    {
        // Prepare base command
        var commandBuilder = new System.Text.StringBuilder();

        // Add source if present (server-side)
        if (!string.IsNullOrEmpty(Source))
        {
            commandBuilder.Append(':').Append(Source).Append(' ');
        }

        // Add AWAY command
        commandBuilder.Append("AWAY");

        // Add message if present
        if (!string.IsNullOrEmpty(Message))
        {
            commandBuilder.Append(" :").Append(Message);
        }

        return commandBuilder.ToString();
    }

    /// <summary>
    /// Creates an AWAY command with no message, indicating the user is no longer away
    /// </summary>
    /// <returns>AWAY command to clear away status</returns>
    public static AwayCommand CreateClear()
    {
        return new AwayCommand();
    }

    /// <summary>
    /// Creates an AWAY command with a specific away message
    /// </summary>
    /// <param name="message">The away message explaining the reason for absence</param>
    /// <returns>AWAY command with the specified message</returns>
    public static AwayCommand Create(string message)
    {
        return new AwayCommand
        {
            Message = message
        };
    }

    /// <summary>
    /// Creates an AWAY command from a server with the specified source and message
    /// </summary>
    /// <param name="source">Source of the command (typically a server or user mask)</param>
    /// <param name="message">The away message</param>
    /// <returns>AWAY command with the specified source and message</returns>
    public static AwayCommand CreateFromServer(string source, string message)
    {
        return new AwayCommand
        {
            Source = source,
            Message = message
        };
    }
}
