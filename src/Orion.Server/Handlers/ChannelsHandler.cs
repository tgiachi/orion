using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc.Channels;
using Orion.Core.Server.Events.Irc.Users;
using Orion.Core.Server.Exceptions.Channels;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Interfaces.Listeners.EventBus;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Data.Messages;
using Orion.Irc.Core.Types;

namespace Orion.Server.Handlers;

public class ChannelsHandler
    : BaseIrcCommandListener, IIrcCommandHandler<JoinCommand>, IIrcCommandHandler<PartCommand>,
        IIrcCommandHandler<NamesCommand>, IIrcCommandHandler<ListCommand>,
        IIrcCommandHandler<ModeCommand>,
        IIrcCommandHandler<PrivMsgCommand>, IIrcCommandHandler<TopicCommand>, IEventBusListener<UserQuitEvent>,
        IEventBusListener<JoinRequestEvent>
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
        RegisterCommandHandler<TopicCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<NamesCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<ListCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<ModeCommand>(this, ServerNetworkType.Clients);

        SubscribeToEventBus<UserQuitEvent>(this);
        SubscribeToEventBus<JoinRequestEvent>(this);

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
        foreach (var channel in command.Channels)
        {
            if (!_channelManagerService.ChannelExists(channel))
            {
                await session.SendCommandAsync(
                    ErrNoSuchChannel.Create(ServerHostName, session.NickName, channel)
                );
                continue;
            }


            if (!_channelManagerService.UserInChannel(session, channel))
            {
                await session.SendCommandAsync(
                    ErrNotOnChannel.Create(ServerHostName, session.NickName, channel)
                );
                continue;
            }


            await _channelManagerService.PartChannel(session, channel, command.PartMessage);
        }
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

        await PublishEventAsync(
            new UserPrivateMessageEvent(session.NickName, channel, message, PrivMessageTarget.TargetType.Channel)
        );
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, TopicCommand command
    )
    {
        await _channelManagerService.SetTopic(session, command.Channel, command.Topic);
    }

    public async Task HandleAsync(UserQuitEvent @event, CancellationToken cancellationToken = default)
    {
        var channels = await _channelManagerService.GetChannelsForNickNameAsync(@event.NickName);

        var session = GetSessionByNickName(@event.NickName);

        foreach (var channel in channels)
        {
            var channelData = _channelManagerService.GetChannel(channel);
            channelData.RemoveMember(@event.NickName);

            var partCommand = PartCommand.CreateForChannel(
                session.FullAddress,
                channel,
                @event.Message
            );

            foreach (var member in channelData.GetMemberList())
            {
                var memberSession = GetSessionByNickName(member);
                if (memberSession != null && memberSession != session)
                {
                    await memberSession.SendCommandAsync(partCommand);
                }
            }
        }
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, NamesCommand command
    )
    {
        foreach (var channel in command.Channels)
        {
            var result = await _channelManagerService.GetNamesAsync(session.NickName, channel);

            session.SendCommandAsync(result.ToArray());
        }
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, ListCommand command
    )
    {
        var commands = await _channelManagerService.ListChannelsAsync(
            session.NickName,
            command.Channels.ToArray(),
            command.Query
        );

        foreach (var listCommand in commands)
        {
            await session.SendCommandAsync(listCommand);
        }
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, ModeCommand command
    )
    {
        if (command.TargetType == ModeTargetType.Channel)
        {
            if (!_channelManagerService.ChannelExists(command.Target))
            {
                await session.SendCommandAsync(
                    ErrNoSuchChannel.Create(ServerHostName, session.NickName, command.Target)
                );
                return;
            }

            var channelData = _channelManagerService.GetChannel(command.Target);


            if (command.ModeChanges.Count == 0)
            {
                await session.SendCommandAsync(
                    RplChannelModeIs.Create(
                        ServerHostName,
                        session.NickName,
                        command.Target,
                        channelData.GetModeString()
                    )
                );

                return;
            }

            if (!channelData.GetMembership(session.NickName).IsOperator && !session.IsOperator)
            {
                await session.SendCommandAsync(
                    ErrChanOpPrivsNeeded.Create(
                        ServerHostName,
                        session.NickName,
                        command.Target
                    )
                );

                return;
            }


            foreach (var modeChange in command.ModeChanges)
            {
                channelData.SetMode(modeChange);
            }

            var modeCommand = ModeCommand.CreateWithModes(
                command.Target,
                channelData.Name,
                command.ModeChanges.ToArray()
            );

            foreach (var member in channelData.GetMemberList())
            {
                var memberSession = GetSessionByNickName(member);
                if (memberSession != null)
                {
                    await memberSession.SendCommandAsync(modeCommand);
                }
            }
        }
    }

    public async Task HandleAsync(JoinRequestEvent @event, CancellationToken cancellationToken = default)
    {
        var session = GetSessionByNickName(@event.Nickname);

        if (session == null)
        {
            Logger.LogWarning("Session not found for nickname: {NickName}", @event.Nickname);
            return;
        }

        await OnCommandReceivedAsync(session, ServerNetworkType.Clients, JoinCommand.Create(@event.Channels));
    }
}
