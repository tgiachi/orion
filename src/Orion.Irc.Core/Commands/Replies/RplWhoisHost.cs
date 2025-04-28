using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents the RPL_WHOISHOST (378) numeric reply showing the host information of a user.
/// This is part of the modern WHOIS command response.
/// </summary>
public class RplWhoisHost : BaseIrcCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RplWhoisHost"/> class.
    /// </summary>
    public RplWhoisHost() : base("378")
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
    /// Gets or sets the host information message.
    /// </summary>
    public string HostInfo { get; set; }

    /// <summary>
    /// Gets or sets the real hostname of the user.
    /// </summary>
    public string Hostname { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the user.
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Parses a raw IRC message line.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    public override void Parse(string line)
    {
        // Format: ":server 378 nickname targetNick :is connecting from *@hostname 192.168.1.1"
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "378"
        Nickname = parts[2];
        TargetNick = parts[3];

        // Get the full host info message
        HostInfo = parts[4].TrimStart(':');

        // Try to extract hostname and IP if the format matches
        if (HostInfo.Contains("is connecting from") && HostInfo.Contains('@'))
        {
            try
            {
                var hostPart = HostInfo.Substring(HostInfo.IndexOf('@') + 1);
                var hostParts = hostPart.Split(' ', 2);

                if (hostParts.Length == 2)
                {
                    Hostname = hostParts[0];
                    IpAddress = hostParts[1];
                }
            }
            catch
            {
                // If extraction fails, just keep the full message
            }
        }
    }

    /// <summary>
    /// Converts the command to its string representation.
    /// </summary>
    /// <returns>The formatted RPL_WHOISHOST string.</returns>
    public override string Write()
    {
        return $":{ServerName} 378 {Nickname} {TargetNick} :{HostInfo}";
    }

    /// <summary>
    /// Creates an RPL_WHOISHOST reply with a custom host info message.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="hostInfo">The host information message.</param>
    /// <returns>A new RPL_WHOISHOST instance.</returns>
    public static RplWhoisHost Create(string serverName, string nickname, string targetNick, string hostInfo)
    {
        return new RplWhoisHost
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            HostInfo = hostInfo
        };
    }

    /// <summary>
    /// Creates an RPL_WHOISHOST reply with hostname and IP address.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="hostname">The hostname of the user.</param>
    /// <param name="ipAddress">The IP address of the user.</param>
    /// <param name="connectingFrom">Whether to include "is connecting from" in the message.</param>
    /// <returns>A new RPL_WHOISHOST instance.</returns>
    public static RplWhoisHost CreateWithHostAndIp(
        string serverName,
        string nickname,
        string targetNick,
        string hostname,
        string ipAddress,
        bool connectingFrom = true)
    {
        string prefix = connectingFrom ? "is connecting from *@" : "is using host *@";
        string hostInfo = $"{prefix}{hostname} {ipAddress}";

        return new RplWhoisHost
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            HostInfo = hostInfo,
            Hostname = hostname,
            IpAddress = ipAddress
        };
    }
}
