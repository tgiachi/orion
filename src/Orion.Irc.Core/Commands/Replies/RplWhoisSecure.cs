using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents the RPL_WHOISSECURE (671) numeric reply indicating a user is connected via a secure connection.
/// This is part of the extended WHOIS command response in modern IRC servers.
/// </summary>
public class RplWhoisSecure : BaseIrcCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RplWhoisSecure"/> class.
    /// </summary>
    public RplWhoisSecure() : base("671")
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
    /// Gets or sets the security information message.
    /// </summary>
    public string SecureInfo { get; set; } = "is using a secure connection";

    /// <summary>
    /// Parses a raw IRC message line.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    public override void Parse(string line)
    {
        // Format: ":server 671 nickname targetNick :is using a secure connection"
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "671"
        Nickname = parts[2];
        TargetNick = parts[3];
        SecureInfo = parts[4].TrimStart(':');
    }

    /// <summary>
    /// Converts the command to its string representation.
    /// </summary>
    /// <returns>The formatted RPL_WHOISSECURE string.</returns>
    public override string Write()
    {
        return $":{ServerName} 671 {Nickname} {TargetNick} :{SecureInfo}";
    }

    /// <summary>
    /// Creates an RPL_WHOISSECURE reply with the default message.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <returns>A new RPL_WHOISSECURE instance.</returns>
    public static RplWhoisSecure Create(string serverName, string nickname, string targetNick)
    {
        return new RplWhoisSecure
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick
        };
    }

    /// <summary>
    /// Creates an RPL_WHOISSECURE reply with a custom security information message.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="secureInfo">Custom security information message.</param>
    /// <returns>A new RPL_WHOISSECURE instance.</returns>
    public static RplWhoisSecure Create(string serverName, string nickname, string targetNick, string secureInfo)
    {
        return new RplWhoisSecure
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            SecureInfo = secureInfo
        };
    }

    /// <summary>
    /// Creates an RPL_WHOISSECURE reply with SSL/TLS protocol information.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="protocol">The SSL/TLS protocol being used (e.g., "TLSv1.3").</param>
    /// <param name="cipher">The cipher being used (e.g., "ECDHE-RSA-AES256-GCM-SHA384").</param>
    /// <returns>A new RPL_WHOISSECURE instance.</returns>
    public static RplWhoisSecure CreateWithProtocolInfo(
        string serverName,
        string nickname,
        string targetNick,
        string protocol,
        string cipher = null)
    {
        string secureInfo = "is using a secure connection";

        if (!string.IsNullOrEmpty(protocol))
        {
            secureInfo += $" via {protocol}";

            if (!string.IsNullOrEmpty(cipher))
            {
                secureInfo += $" ({cipher})";
            }
        }

        return new RplWhoisSecure
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            SecureInfo = secureInfo
        };
    }
}
