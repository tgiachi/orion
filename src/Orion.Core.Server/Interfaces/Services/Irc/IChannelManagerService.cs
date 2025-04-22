using Orion.Core.Server.Data.Channels;
using Orion.Core.Server.Data.Sessions;
using Orion.Irc.Core.Data.Channels;

namespace Orion.Core.Server.Interfaces.Services.Irc;

public interface IChannelManagerService
{
    bool ChannelExists(string channelName);


    ChannelData? GetChannel(string channelName);

    Task<ChannelJoinResult> JoinChannelAsync(IrcUserSession session, string channelName, string channelPassword = null);

    Task<List<ChannelTopicEntry>> GetChannelTopicsAsync(bool hidePrivateChannels = true);

    bool UserInChannel(IrcUserSession session, string channelName);

}
