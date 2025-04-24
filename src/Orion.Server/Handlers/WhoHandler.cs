using System.Text;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Data.Channels;

namespace Orion.Server.Handlers;

public class WhoHandler : BaseIrcCommandListener, IIrcCommandHandler<WhoCommand>, IIrcCommandHandler<WhoIsCommand>
{
    private readonly IChannelManagerService _channelManagerService;

    public WhoHandler(
        ILogger<WhoHandler> logger, IrcCommandListenerContext context, IChannelManagerService channelManagerService
    ) : base(logger, context)
    {
        _channelManagerService = channelManagerService;
        RegisterCommandHandler<WhoCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<WhoIsCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, WhoCommand command)
    {
        if (command.IsChannel)
        {
            await WhoChannel(session, command.Mask);
        }
        else
        {
            await WhoUser(session, command.Mask);
        }

        await session.SendCommandAsync(
            RplEndOfWho.Create(
                ServerHostName,
                session.NickName,
                command.Mask
            )
        );
    }

    private async Task WhoChannel(IrcUserSession session, string channelName)
    {
        var channel = _channelManagerService.GetChannel(channelName);

        if (channel == null)
        {
            await session.SendCommandAsync(
                ErrNoSuchChannel.Create(
                    ServerHostName,
                    session.NickName,
                    channelName
                )
            );
            return;
        }

        foreach (var member in _channelManagerService.GetChannel(channelName).Members)
        {
            var userSession = GetSessionByNickName(member.NickName);
            if (userSession == null)
            {
                continue;
            }


            await session.SendCommandAsync(
                RplWhoReply.Create(
                    ServerHostName,
                    session.NickName,
                    channelName,
                    userSession.UserName,
                    userSession.VHostName ?? userSession.HostName,
                    Config.Server.Host,
                    userSession.NickName,
                    GetUserFlags(session, channel, member),
                    0,
                    session.RealName
                )
            );
        }
    }

    private static string GetUserFlags(IrcUserSession session, ChannelData channel, ChannelMembership member)
    {
        var flags = new StringBuilder();

        if (member.IsOperator)
        {
            flags.Append('@');
        }

        if (member.HasVoice)
        {
            flags.Append('+');
        }

        flags.Append(session.IsAway ? 'G' : 'H');

        if (session.IsInvisible)
        {
            flags.Append('i');
        }

        if (session.IsRegistered)
        {
            flags.Append('r');
        }

        return flags.ToString();
    }

    private async Task WhoUser(IrcUserSession session, string nickName)
    {
        var userSessions = QuerySessions(s => s.IsRegistered && !s.IsInvisible && s.NickName.Equals(nickName, StringComparison.OrdinalIgnoreCase));

        foreach (var userSession in userSessions)
        {
            await session.SendCommandAsync(
                RplWhoReply.Create(
                    ServerHostName,
                    session.NickName,
                    "*",
                    userSession.UserName,
                    userSession.VHostName ?? userSession.HostName,
                    Config.Server.Host,
                    userSession.NickName,
                    session.IsAway ? "G" : "H",
                    0,
                    session.RealName
                )
            );
        }
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, WhoIsCommand command
    )
    {
    }
}
