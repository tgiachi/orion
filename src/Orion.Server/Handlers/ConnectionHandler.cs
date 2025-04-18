using HyperCube.Postman.Interfaces.Services;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Foundations.Types;
using Orion.Foundations.Utils;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Data.Messages;

namespace Orion.Server.Handlers;

public class ConnectionHandler
    : BaseIrcCommandListener, IIrcCommandHandler<CapCommand>, IIrcCommandHandler<UserCommand>,
        IIrcCommandHandler<NickCommand>, ILetterListener<SessionConnectedEvent>
{
    public ConnectionHandler(ILogger<ConnectionHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        SubscribeToPostman(this);

        RegisterCommandHandler<NickCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<UserCommand>(this, ServerNetworkType.Clients);
    }

    public Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, CapCommand command)
    {
        return Task.CompletedTask;
    }

    public Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, UserCommand command)
    {
        session.RealName = command.RealName;
        session.UserName = command.UserName;

        session.IsUserSent = true;

        return Task.CompletedTask;
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, NickCommand command
    )
    {
        var exists = QuerySessions(s => s.NickName.Equals(command.Nickname, StringComparison.OrdinalIgnoreCase));

        if (exists.Count == 0)
        {
            session.NickName = command.Nickname;
            session.IsNickSent = true;
        }
        else
        {
            Logger.LogWarning("Nickname already in use: {Nickname}", command.Nickname);

            await session.SendCommandAsync(
                ErrNicknameInUse.CreateForUnregistered(ServerContextData.ServerName, command.Nickname)
            );
        }
    }

    public async Task HandleAsync(SessionConnectedEvent @event, CancellationToken cancellationToken = default)
    {
        var session = GetSession(@event.SessionId);

        // Check ident

        await session.SendCommandAsync(
            NoticeCommand.CreateFromServer(ServerContextData.ServerName, "*", ServerNotices.Connection.CheckingForClones)
        );

        await session.SendCommandAsync(
            NoticeCommand.CreateFromServer(ServerContextData.ServerName, "*", ServerNotices.Connection.LookingUpHostname)
        );

        var addressFound = await DnsUtils.TryResolveHostnameAsync(session.RemoteAddress);

        if (addressFound.Resolved)
        {
            await session.SendCommandAsync(
                NoticeCommand.CreateFromServer(
                    ServerContextData.ServerName,
                    "*",
                    ServerNotices.Connection.ConnectingToHost + " " + addressFound.HostName
                )
            );
        }
        else
        {
            await session.SendCommandAsync(
                NoticeCommand.CreateFromServer(
                    ServerContextData.ServerName,
                    "*",
                    ServerNotices.Connection.HostnameLookupFailed
                )
            );
        }

        session.HostName = addressFound.HostName;
    }
}
