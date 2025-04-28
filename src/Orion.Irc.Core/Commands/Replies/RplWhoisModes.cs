using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents the RPL_WHOISMODES (379) numeric reply showing the user modes of a user.
/// This is part of the extended WHOIS command response in some IRC servers.
/// </summary>
public class RplWhoisModes : BaseIrcCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RplWhoisModes"/> class.
    /// </summary>
    public RplWhoisModes() : base("379")
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
    /// Gets or sets the modes message.
    /// </summary>
    public string ModesMessage { get; set; }

    /// <summary>
    /// Gets or sets the actual user modes string.
    /// </summary>
    public string Modes { get; set; }

    /// <summary>
    /// Parses a raw IRC message line.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    public override void Parse(string line)
    {
        // Format: ":server 379 nickname targetNick :is using modes +iwxz"
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "379"
        Nickname = parts[2];
        TargetNick = parts[3];

        // Get the full modes message
        ModesMessage = parts[4].TrimStart(':');

        // Try to extract the actual modes if possible
        if (ModesMessage.Contains("is using modes +"))
        {
            try
            {
                int modesIndex = ModesMessage.IndexOf("is using modes +") + "is using modes ".Length;
                Modes = ModesMessage.Substring(modesIndex);
            }
            catch
            {
                // If extraction fails, leave Modes as null
            }
        }
    }

    /// <summary>
    /// Converts the command to its string representation.
    /// </summary>
    /// <returns>The formatted RPL_WHOISMODES string.</returns>
    public override string Write()
    {
        return $":{ServerName} 379 {Nickname} {TargetNick} :{ModesMessage}";
    }

    /// <summary>
    /// Creates an RPL_WHOISMODES reply with a custom modes message.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="modesMessage">The complete modes message.</param>
    /// <returns>A new RPL_WHOISMODES instance.</returns>
    public static RplWhoisModes Create(string serverName, string nickname, string targetNick, string modesMessage)
    {
        return new RplWhoisModes
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            ModesMessage = modesMessage
        };
    }

    /// <summary>
    /// Creates an RPL_WHOISMODES reply from user modes.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="modes">The user modes (e.g., "+iwxz").</param>
    /// <returns>A new RPL_WHOISMODES instance.</returns>
    public static RplWhoisModes CreateFromModes(string serverName, string nickname, string targetNick, string modes)
    {
        // Ensure modes starts with + if it's not empty and doesn't already have a prefix
        if (!string.IsNullOrEmpty(modes) && !modes.StartsWith("+") && !modes.StartsWith("-"))
        {
            modes = "+" + modes;
        }

        string modesMessage = $"is using modes {modes}";

        return new RplWhoisModes
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            ModesMessage = modesMessage,
            Modes = modes
        };
    }
}
