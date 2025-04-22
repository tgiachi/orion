using Orion.Core.Server.Data.Channels;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Irc.Core.Data.Channels;

namespace Orion.Core.Server.Interfaces.Services.Irc;

public interface IChannelManagerService : IOrionService
{
    bool ChannelExists(string channelName);

    ChannelData? GetChannel(string channelName);

    Task<ChannelJoinResult> JoinChannelAsync(IrcUserSession session, string channelName, string? channelPassword = null);

    Task<List<ChannelTopicEntry>> GetChannelTopicsAsync(bool hidePrivateChannels = true);

    bool UserInChannel(IrcUserSession session, string channelName);
    List<string> GetUsersInChannel(string channelName);

    Task<bool> PartChannel(IrcUserSession session, string channelName, string? partMessage = null);

    Task<bool> PartChannel(string nickName, string channelName, string? partMessage = null);


}
