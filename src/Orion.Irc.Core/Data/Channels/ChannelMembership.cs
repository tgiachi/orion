namespace Orion.Irc.Core.Data.Channels;

/// <summary>
/// Represents a client's membership status in a channel
/// </summary>
public class ChannelMembership
{

    /// <summary>
    ///  Member's nickname in the channel
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// Whether the client has operator status (+o) in the channel
    /// </summary>
    public bool IsOperator { get; set; }

    /// <summary>
    /// Whether the client has voice (+v) in the channel
    /// </summary>
    public bool HasVoice { get; set; }

    /// <summary>
    /// When the client joined the channel
    /// </summary>
    public DateTime JoinTime { get; }

    public ChannelMembership()
    {
        JoinTime = DateTime.UtcNow;
    }

    public string PrefixNickname => IsOperator ? $"@{NickName}" : HasVoice ? $"+{NickName}" : NickName;
}
