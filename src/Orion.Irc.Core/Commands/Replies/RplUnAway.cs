using Orion.Irc.Core.Commands.Base;


namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents RPL_UNAWAY (305) numeric reply sent when a user is no longer marked as away
/// </summary>
public class RplUnaway : BaseIrcCommand
{
    /// <summary>
    /// Initializes a new instance of the RPL_UNAWAY numeric reply
    /// </summary>
    public RplUnaway() : base("305")
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
    /// The message indicating the user is no longer away
    /// </summary>
    public string Message { get; set; } = "You are no longer marked as being away";

    /// <summary>
    /// Parses a raw IRC message to extract RPL_UNAWAY data
    /// </summary>
    /// <param name="line">Raw IRC message</param>
    public override void Parse(string line)
    {
        // Example: ":server.com 305 nickname :You are no longer marked as being away"
        var parts = line.Split(' ', 4);

        if (parts.Length < 4)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "305"
        Nickname = parts[2];
        Message = parts[3].TrimStart(':');
    }

    /// <summary>
    /// Converts the command to its string representation
    /// </summary>
    /// <returns>Formatted RPL_UNAWAY string</returns>
    public override string Write()
    {
        return $":{ServerName} 305 {Nickname} :{Message}";
    }

    /// <summary>
    /// Creates an RPL_UNAWAY reply
    /// </summary>
    /// <param name="serverName">The server name sending the reply</param>
    /// <param name="nickname">The client nickname receiving the reply</param>
    /// <param name="message">The away message (usually "You are no longer marked as being away")</param>
    /// <returns>An RPL_UNAWAY command instance</returns>
    public static RplUnaway Create(string serverName, string nickname, string message = null)
    {
        return new RplUnaway
        {
            ServerName = serverName,
            Nickname = nickname,
            Message = message ?? "You are no longer marked as being away"
        };
    }
}
