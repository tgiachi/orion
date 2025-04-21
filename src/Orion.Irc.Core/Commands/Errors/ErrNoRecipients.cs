using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Errors;

/// <summary>
/// Represents the ERR_NORECIPIENT (411) numeric error
/// Returned when a client tries to send a message without specifying a recipient
/// </summary>
public class ErrNoRecipients : BaseIrcCommand
{
    /// <summary>
    /// The server name/source of the error
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// The nickname of the client receiving this error
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// The command that was attempted (e.g., "PRIVMSG")
    /// </summary>
    public string Command { get; set; }

    /// <summary>
    /// The error message
    /// </summary>
    public string Message { get; set; } = "No recipient given";

    /// <summary>
    /// Creates a new instance of ErrNoRecipients
    /// </summary>
    public ErrNoRecipients() : base("411")
    {
    }

    /// <summary>
    /// Parses an ERR_NORECIPIENT message from a raw IRC line
    /// </summary>
    /// <param name="line">The raw IRC message line</param>
    public override void Parse(string line)
    {
        // Format: ":server.com 411 nickname :No recipient given (PRIVMSG)"
        var parts = line.Split(' ', 4);

        if (parts.Length < 4)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "411"
        Nickname = parts[2];

        // The message part with command
        string messagePart = parts[3].TrimStart(':');

        // Extract command if present in parentheses
        int openParenIndex = messagePart.IndexOf('(');
        int closeParenIndex = messagePart.IndexOf(')');

        if (openParenIndex != -1 && closeParenIndex > openParenIndex)
        {
            Command = messagePart.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
            // Extract message without the command part
            Message = messagePart.Substring(0, openParenIndex).Trim();
        }
        else
        {
            Message = messagePart;
        }
    }

    /// <summary>
    /// Converts the command to its string representation
    /// </summary>
    /// <returns>The IRC protocol representation of the command</returns>
    public override string Write()
    {
        // Format: ":server.com 411 nickname :No recipient given (PRIVMSG)"
        string formattedMessage = string.IsNullOrEmpty(Command)
            ? Message
            : $"{Message} ({Command})";

        return $":{ServerName} 411 {Nickname} :{formattedMessage}";
    }

    /// <summary>
    /// Creates an ERR_NORECIPIENT error for a specific command
    /// </summary>
    /// <param name="serverName">The server name</param>
    /// <param name="nickname">The nickname of the target client</param>
    /// <param name="command">The command that was attempted (e.g., "PRIVMSG")</param>
    /// <returns>A new ERR_NORECIPIENT error</returns>
    public static ErrNoRecipients Create(string serverName, string nickname, string command)
    {
        return new ErrNoRecipients
        {
            ServerName = serverName,
            Nickname = nickname,
            Command = command,
            Message = "No recipient given"
        };
    }

    /// <summary>
    /// Creates an ERR_NORECIPIENT error with a custom message
    /// </summary>
    /// <param name="serverName">The server name</param>
    /// <param name="nickname">The nickname of the target client</param>
    /// <param name="command">The command that was attempted (e.g., "PRIVMSG")</param>
    /// <param name="message">Custom error message</param>
    /// <returns>A new ERR_NORECIPIENT error</returns>
    public static ErrNoRecipients Create(string serverName, string nickname, string command, string message)
    {
        return new ErrNoRecipients
        {
            ServerName = serverName,
            Nickname = nickname,
            Command = command,
            Message = message
        };
    }
}
