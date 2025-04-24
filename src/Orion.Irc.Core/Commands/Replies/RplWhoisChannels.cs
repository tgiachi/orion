using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands.Replies;

/// <summary>
/// Represents the RPL_WHOISCHANNELS (319) numeric reply listing the channels a user is on.
/// This is part of the WHOIS command response.
/// </summary>
public class RplWhoisChannels : BaseIrcCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RplWhoisChannels"/> class.
    /// </summary>
    public RplWhoisChannels() : base("319")
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
    /// Gets or sets the list of channels the target user is on.
    /// Each entry may be prefixed with channel status symbols:
    /// @ for channel operator, + for voiced, etc.
    /// </summary>
    public List<string> Channels { get; set; } = new List<string>();

    /// <summary>
    /// Parses a raw IRC message line.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    public override void Parse(string line)
    {
        // Format: ":server 319 nickname targetNick :@#channel1 +#channel2 #channel3"
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
        {
            return; // Invalid format
        }

        ServerName = parts[0].TrimStart(':');
        // parts[1] should be "319"
        Nickname = parts[2];
        TargetNick = parts[3];

        // Extract channel list (removing leading colon)
        var channelList = parts[4].TrimStart(':');

        // Split the channel list by spaces
        if (!string.IsNullOrWhiteSpace(channelList))
        {
            Channels = channelList.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        else
        {
            Channels.Clear();
        }
    }

    /// <summary>
    /// Converts the command to its string representation.
    /// </summary>
    /// <returns>The formatted RPL_WHOISCHANNELS string.</returns>
    public override string Write()
    {
        string channelList = string.Join(" ", Channels);
        return $":{ServerName} 319 {Nickname} {TargetNick} :{channelList}";
    }

    /// <summary>
    /// Creates an RPL_WHOISCHANNELS reply.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="channels">The list of channels the target user is on.</param>
    /// <returns>A new RPL_WHOISCHANNELS instance.</returns>
    public static RplWhoisChannels Create(string serverName, string nickname, string targetNick, IEnumerable<string> channels)
    {
        return new RplWhoisChannels
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            Channels = channels?.ToList() ?? new List<string>()
        };
    }

    /// <summary>
    /// Adds a channel with the appropriate prefix based on the user's status in that channel.
    /// </summary>
    /// <param name="channelName">The name of the channel.</param>
    /// <param name="isOperator">Whether the user is an operator in the channel.</param>
    /// <param name="isVoiced">Whether the user has voice status in the channel.</param>
    /// <returns>The channel name with the appropriate prefix.</returns>
    public static string FormatChannelWithUserStatus(string channelName, bool isOperator, bool isVoiced)
    {
        if (isOperator)
        {
            return "@" + channelName;
        }
        else if (isVoiced)
        {
            return "+" + channelName;
        }

        return channelName;
    }

    /// <summary>
    /// Creates an RPL_WHOISCHANNELS reply with channel and status information.
    /// </summary>
    /// <param name="serverName">The server name.</param>
    /// <param name="nickname">The nickname of the receiving client.</param>
    /// <param name="targetNick">The target nickname being queried.</param>
    /// <param name="channelStatusPairs">Dictionary of channels and user status flags (isOperator, isVoiced).</param>
    /// <returns>A new RPL_WHOISCHANNELS instance.</returns>
    public static RplWhoisChannels CreateWithStatus(
        string serverName,
        string nickname,
        string targetNick,
        Dictionary<string, (bool isOperator, bool isVoiced)> channelStatusPairs)
    {
        var formattedChannels = channelStatusPairs.Select(pair =>
            FormatChannelWithUserStatus(pair.Key, pair.Value.isOperator, pair.Value.isVoiced)).ToList();

        return new RplWhoisChannels
        {
            ServerName = serverName,
            Nickname = nickname,
            TargetNick = targetNick,
            Channels = formattedChannels
        };
    }
}
