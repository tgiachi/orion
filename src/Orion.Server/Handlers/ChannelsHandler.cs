using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Exceptions.Channels;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;

namespace Orion.Server.Handlers;

public class ChannelsHandler
    : BaseIrcCommandListener, IIrcCommandHandler<JoinCommand>, IIrcCommandHandler<PartCommand>,
        IIrcCommandHandler<PrivMsgCommand>
{
    private readonly IChannelManagerService _channelManagerService;


    public ChannelsHandler(
        ILogger<ChannelsHandler> logger, IrcCommandListenerContext context, IChannelManagerService channelManagerService
    ) : base(logger, context)
    {
        _channelManagerService = channelManagerService;
        RegisterCommandHandler<JoinCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<PartCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<PrivMsgCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, JoinCommand command
    )
    {
        foreach (var channel in command.Channels)
        {
            var result = await _channelManagerService.JoinChannelAsync(session, channel.ChannelName, channel.Key);

            if (result.Exception is ChannelFullException)
            {
                await session.SendCommandAsync(
                    ErrChannelIsFull.Create(ServerHostName, session.NickName, channel.ChannelName)
                );
                continue;
            }

            if (result.Exception is ChannelInvalidPasswordException)
            {
                await session.SendCommandAsync(
                    ErrBadChannelKey.Create(ServerHostName, session.NickName, channel.ChannelName)
                );
                continue;
            }

            foreach (var sessionCommand in result.JoinedUserCommands)
            {
                await session.SendCommandAsync(sessionCommand);
            }

            foreach (var member in result.MembersCommands)
            {
                var memberSession = GetSessionByNickName(member.Key);
                if (memberSession != null)
                {
                    foreach (var memberCommand in member.Value)
                    {
                        await memberSession.SendCommandAsync(memberCommand);
                    }
                }
            }
        }
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, PartCommand command
    )
    {
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, PrivMsgCommand command
    )
    {
        if (command.Targets.Length == 0)
        {
            return;
        }

        var channels = command.Targets.Where(s => s.IsChannel).ToList();


        foreach (var channel in channels)
        {
            await SendMessageToChannel(session, channel.Target, command.Message);
        }
    }


    private async Task SendMessageToChannel(IrcUserSession session, string channel, string message)
    {
        if (!_channelManagerService.ChannelExists(channel))
        {
            session.SendCommandAsync(
                ErrNoSuchChannel.Create(ServerHostName, session.NickName, channel)
            );
            return;
        }

        var channelData = _channelManagerService.GetChannel(channel);

        if (!channelData.UserCanSendMessage(session.NickName))
        {
            session.SendCommandAsync(
                ErrCannotSendToChan.Create(ServerHostName, session.NickName, channel)
            );
            return;
        }

        var messageCommand = PrivMsgCommand.CreateToChannel(
            session.FullAddress,
            channel,
            message
        );

        foreach (var memberNickName in channelData.GetMemberList())
        {
            var memberSession = GetSessionByNickName(memberNickName);
            if (memberSession != null && memberSession != session)
            {
                await memberSession.SendCommandAsync(messageCommand);
            }
        }
    }
}
