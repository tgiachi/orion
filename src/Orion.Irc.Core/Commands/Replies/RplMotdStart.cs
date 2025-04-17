using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
///     Represents the RPL_MOTDSTART (375) numeric reply that indicates the start of MOTD
/// </summary>
public class RplMotdStart : BaseIrcCommand
{
    public RplMotdStart() : base("375")
    {
    }


    public RplMotdStart(string serverName, string nickname) : base("375")
    {
        ServerName = serverName;
        Nickname = nickname;
    }

    /// <summary>
    ///     The nickname of the client receiving this reply
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    ///     The server name sending this reply
    /// </summary>
    public string ServerName { get; set; }

    public override void Parse(string line)
    {
        // Example: :server.com 375 nickname :- server.com Message of the Day -
        var parts = line.Split(' ', 4);

        if (parts.Length < 4)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "375"
        Nickname = parts[2];
    }

    public override string Write()
    {
        return $":{ServerName} 375 {Nickname} :- {ServerName} Message of the Day -";
    }

    /// <summary>
    ///     Creates a RPL_MOTDSTART reply
    /// </summary>
    public static RplMotdStart Create(string serverName, string nickname)
    {
        return new RplMotdStart
        {
            ServerName = serverName,
            Nickname = nickname
        };
    }
}
