using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents RPL_NOWAWAY (306) numeric reply sent when a user has been marked as away
/// </summary>
public class RplNowaway : BaseIrcCommand
{
    /// <summary>
    /// Initializes a new instance of the RPL_NOWAWAY numeric reply
    /// </summary>
    public RplNowaway() : base("306")
    {
    }

    /// <summary>
    /// The server name sending this reply
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// The nickname of the client receiving this reply
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// The message indicating the user is now away
    /// </summary>
    public string Message { get; set; } = "You have been marked as being away";

    /// <summary>
    /// Parses a raw IRC message to extract RPL_NOWAWAY data
    /// </summary>
    /// <param name="line">Raw IRC message</param>
    public override void Parse(string line)
    {
        // Example: ":server.com 306 nickname :You have been marked as being away"
        var parts = line.Split(' ', 4);

        if (parts.Length < 4)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "306"
        Nickname = parts[2];
        Message = parts[3].TrimStart(':');
    }

    /// <summary>
    /// Converts the command to its string representation
    /// </summary>
    /// <returns>Formatted RPL_NOWAWAY string</returns>
    public override string Write()
    {
        return $":{ServerName} 306 {Nickname} :{Message}";
    }

    /// <summary>
    /// Creates an RPL_NOWAWAY reply
    /// </summary>
    /// <param name="serverName">The server name sending the reply</param>
    /// <param name="nickname">The client nickname receiving the reply</param>
    /// <param name="message">The away message (usually "You have been marked as being away")</param>
    /// <returns>An RPL_NOWAWAY command instance</returns>
    public static RplNowaway Create(string serverName, string nickname, string message = null)
    {
        return new RplNowaway
        {
            ServerName = serverName,
            Nickname = nickname,
            Message = message ?? "You have been marked as being away"
        };
    }
}
