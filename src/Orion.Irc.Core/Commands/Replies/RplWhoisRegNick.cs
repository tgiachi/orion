using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents the RPL_WHOISREGNICK (307) numeric reply that indicates a nickname is registered.
/// Commonly used in UnrealIRCd and other servers with services integration.
/// </summary>
public class RplWhoisRegNick : BaseIrcCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RplWhoisRegNick"/> class.
    /// </summary>
    public RplWhoisRegNick() : base("307")
    {
    }

    /// <summary>
    /// Gets or sets the server name sending this reply.
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// Gets or sets the nickname of the client receiving this reply.
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// Gets or sets the target nickname that is being queried.
    /// </summary>
    public string TargetNick { get; set; }

    /// <summary>
    /// Gets or sets the message about registration status.
    /// </summary>
    public string Message { get; set; } = "is a registered nick";

    /// <summary>
    /// Parses a raw IRC message line.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    public override void Parse(string line)
    {
        // Format: ":server 307 nickname targetNick :is a registered nick"
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "307"
        Nickname = parts[2];
        TargetNick = parts[3];
        Message = parts[4].TrimStart(':');
    }

    /// <summary>
    /// Converts the command to its string representation.
    /// </summary>
    /// <returns>The formatted RPL_WHOISREGNICK string.</returns>
    public override string Write()
    {
        return $":{ServerName} 307 {Nickname} {TargetNick} :{Message}";
    }

    /// <summary>
    /// Creates an RPL_WHOISREGNICK reply with the default message.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname that is registered.</param>
    /// <returns>A new RPL_WHOISREGNICK instance.</returns>
    public static RplWhoisRegNick Create(string serverName, string nickname, string targetNick)
    {
        return new RplWhoisRegNick
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick
        };
    }

    /// <summary>
    /// Creates an RPL_WHOISREGNICK reply with a custom message.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname that is registered.</param>
    /// <param name="message">Custom message about registration status.</param>
    /// <returns>A new RPL_WHOISREGNICK instance.</returns>
    public static RplWhoisRegNick Create(string serverName, string nickname, string targetNick, string message)
    {
        return new RplWhoisRegNick
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            Message = message
        };
    }
}
