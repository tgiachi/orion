using Orion.Core.Irc.Server.Data.Channels;
using Orion.Core.Irc.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Irc.Core.Data.Channels;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Irc.Server.Interfaces.Services.Irc;

public interface IChannelManagerService : IOrionService
{
    List<ChannelData> Channels { get; }

    bool ChannelExists(string channelName);

    ChannelData? GetChannel(string channelName);

    Task<ChannelJoinResult> JoinChannelAsync(IrcUserSession session, string channelName, string? channelPassword = null);

    Task<List<ChannelTopicEntry>> GetChannelTopicsAsync(bool hidePrivateChannels = true);

    bool UserInChannel(IrcUserSession session, string channelName);
    List<string> GetUsersInChannel(string channelName);

    Task<bool> PartChannel(IrcUserSession session, string channelName, string? partMessage = null);

    Task<bool> PartChannel(string nickName, string channelName, string? partMessage = null);


    Task<bool> SetTopic(IrcUserSession session, string channelName, string topicName);
    Task<bool> SetTopic(string nickName, string channelName, string topicName);


    Task<List<string>> GetChannelsForNickNameAsync(string nickName);

    Task<List<IIrcCommand>> GetNamesAsync(string nickName, string channelName);

    Task<List<IIrcCommand>> GetTopicsAsync(string nickName, string channelName);

    Task<List<IIrcCommand>> ListChannelsAsync(string nickName, string[] channels = null, string query = null);

    Task<List<string>> GetConnectedUsersAsync(string nickName);
    void UpdateNickName(string oldNickName, string newNickName);
}
