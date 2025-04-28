using System.Text;
using Orion.Core.Irc.Server.Data.Internal;
using Orion.Core.Irc.Server.Data.Sessions;
using Orion.Core.Irc.Server.Handlers.Base;
using Orion.Core.Irc.Server.Interfaces.Listeners.Commands;
using Orion.Core.Irc.Server.Interfaces.Services.Irc;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Data.Channels;

namespace Orion.Server.Handlers;

public class WhoHandler : BaseIrcCommandListener, IIrcCommandHandler<WhoCommand>, IIrcCommandHandler<WhoIsCommand>
{
    private readonly IChannelManagerService _channelManagerService;
    private readonly IVersionService _versionService;

    public WhoHandler(
        ILogger<WhoHandler> logger, IrcCommandListenerContext context, IChannelManagerService channelManagerService,
        IVersionService versionService
    ) : base(logger, context)
    {
        _channelManagerService = channelManagerService;
        _versionService = versionService;
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
        var userSessions = QuerySessions(s =>
            s.IsRegistered && !s.IsInvisible && s.NickName.Equals(nickName, StringComparison.OrdinalIgnoreCase)
        );

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
        if (command.Nicknames.Count == 0)
        {
            await session.SendCommandAsync(
                ErrNoSuchNick.Create(
                    ServerHostName,
                    session.NickName,
                    command.Nicknames.FirstOrDefault() ?? string.Empty
                )
            );
        }

        foreach (var nickName in command.Nicknames)
        {
            var targetSession = GetSessionByNickName(nickName);

            if (targetSession != null)
            {
                await WhoIsUser(session, targetSession);
            }
        }


        await session.SendCommandAsync(
            RplEndOfWho.Create(
                ServerHostName,
                session.NickName,
                string.Join(',', command.Nicknames)
            )
        );
    }


    private async Task WhoIsUser(IrcUserSession source, IrcUserSession targetSession)
    {
        await source.SendCommandAsync(RplWhoisRegNick.Create(ServerHostName, source.NickName, targetSession.NickName));
        await source.SendCommandAsync(
            RplWhoisUser.Create(
                ServerHostName,
                source.NickName,
                targetSession.NickName,
                targetSession.UserName,
                targetSession.VHostName ?? targetSession.HostName,
                targetSession.RealName
            )
        );

        await source.SendCommandAsync(
            RplWhoisServer.Create(
                ServerHostName,
                source.NickName,
                targetSession.NickName,
                Config.Server.Host,
                _versionService.GetVersionInfo().AppName + " v" + _versionService.GetVersionInfo().Version
            )
        );

        if (targetSession.IsOperator)
        {
            await source.SendCommandAsync(
                RplWhoisOperator.Create(
                    ServerHostName,
                    source.NickName,
                    targetSession.NickName
                )
            );
        }

        await source.SendCommandAsync(
            RplWhoisIdle.Create(
                ServerHostName,
                source.NickName,
                targetSession.NickName,
                (int)(DateTime.Now - targetSession.LastActivity).TotalSeconds,
                targetSession.Created
            )
        );


        var channels = await _channelManagerService.GetChannelsForNickNameAsync(targetSession.NickName);

        foreach (var channel in channels)
        {
            var channelData = _channelManagerService.GetChannel(channel);

            if (channelData == null)
            {
                continue;
            }

            var channelMemberShip = channelData.GetMembership(targetSession.NickName);

            await source.SendCommandAsync(
                RplWhoisChannels.CreateWithStatus(
                    ServerHostName,
                    source.NickName,
                    targetSession.NickName,
                    new Dictionary<string, (bool isOperator, bool isVoiced)>()
                    {
                        { channelData.Name, (channelMemberShip.IsOperator, channelMemberShip.HasVoice) }
                    }
                )
            );
        }

        var ipAddress = targetSession.RemoteAddress;

        if (targetSession.IsOperator)
        {
            ipAddress = "0.0.0.0";
        }

        await source.SendCommandAsync(
            RplWhoisHost.CreateWithHostAndIp(
                ServerHostName,
                source.NickName,
                targetSession.HostName,
                targetSession.VHostName ?? targetSession.HostName,
                ipAddress
            )
        );


        await source.SendCommandAsync(
            RplWhoisModes.CreateFromModes(
                ServerHostName,
                source.NickName,
                targetSession.NickName,
                targetSession.ModesString
            )
        );

        if (targetSession.IsSecureConnection)
        {
            await source.SendCommandAsync(
                RplWhoisSecure.Create(
                    ServerHostName,
                    source.NickName,
                    targetSession.NickName
                )
            );
        }

        if (targetSession.IsAway)
        {
            await source.SendCommandAsync(
                RplAway.Create(
                    ServerHostName,
                    source.NickName,
                    targetSession.NickName,
                    targetSession.AwayMessage
                )
            );
        }
    }


}
