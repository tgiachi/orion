using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents the RPL_UMODEIS (221) numeric reply showing a user's mode flags
/// </summary>
public class RplUModeIs : BaseIrcCommand
{
    public RplUModeIs() : base("221")
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
    /// The mode string (e.g. "+ix") showing the user's mode flags
    /// </summary>
    public string ModeString { get; set; }

    public override void Parse(string line)
    {
        // Format: ":server.com 221 nickname :+ix"
        var parts = line.Split(' ', 4);

        if (parts.Length < 4)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "221"
        Nickname = parts[2];
        ModeString = parts[3].TrimStart(':');
    }

    public override string Write()
    {
        // Format: ":server.com 221 nickname :+ix"
        return $":{ServerName} 221 {Nickname} :{ModeString}";
    }

    /// <summary>
    /// Creates an RPL_UMODEIS (221) reply
    /// </summary>
    /// <param name="serverName">The server name</param>
    /// <param name="nickname">The target nickname</param>
    /// <param name="modeString">The mode string showing user modes</param>
    /// <returns>A new RplUModeIs instance</returns>
    public static RplUModeIs Create(string serverName, string nickname, string modeString)
    {
        return new RplUModeIs
        {
            ServerName = serverName,
            Nickname = nickname,
            ModeString = modeString
        };
    }
}
