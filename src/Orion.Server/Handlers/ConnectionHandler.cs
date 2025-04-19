
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc;
using Orion.Core.Server.Events.Users;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Interfaces.Listeners.EventBus;
using Orion.Foundations.Types;
using Orion.Foundations.Utils;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Data.Messages;

namespace Orion.Server.Handlers;

public class ConnectionHandler
    : BaseIrcCommandListener, IIrcCommandHandler<CapCommand>, IIrcCommandHandler<UserCommand>,
        IIrcCommandHandler<NickCommand>, IIrcCommandHandler<PassCommand>, IEventBusListener<SessionConnectedEvent>
{
    private readonly bool _isPasswordRequired;

    public ConnectionHandler(ILogger<ConnectionHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        SubscribeToPostman(this);

        RegisterCommandHandler<NickCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<UserCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<PassCommand>(this, ServerNetworkType.Clients);

        if (!string.IsNullOrEmpty(Config.Irc.ServerPassword))
        {
            _isPasswordRequired = true;
        }
    }

    public Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, CapCommand command)
    {
        return Task.CompletedTask;
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, UserCommand command
    )
    {
        session.RealName = command.RealName;
        session.UserName = command.UserName;

        await SendIfAuthenticated(session);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, NickCommand command
    )
    {
        var exists = QuerySessions(s => s.NickName.Equals(command.Nickname, StringComparison.OrdinalIgnoreCase));

        if (exists.Count == 0)
        {
            session.NickName = command.Nickname;
        }
        else
        {
            Logger.LogWarning("Nickname already in use: {Nickname}", command.Nickname);

            await session.SendCommandAsync(
                ErrNicknameInUse.CreateForUnregistered(ServerContextData.ServerName, command.Nickname)
            );
        }

        await SendIfAuthenticated(session);
    }

    private async Task SendIfAuthenticated(IrcUserSession session)
    {
        if (session.IsAuthenticated)
        {
            if (_isPasswordRequired && session.IsPasswordValid)
            {
                await PublishEventAsync(new UserAuthenticatedEvent(session.SessionId));
            }
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

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, PassCommand command
    )
    {
        if (!string.IsNullOrEmpty(Config.Irc.ServerPassword) && _isPasswordRequired)
        {
            if (command.Password != Config.Irc.ServerPassword)
            {
                Logger.LogWarning("Invalid password for session {SessionId}", session.SessionId);

                await session.SendCommandAsync(ErrPasswdMismatch.Create(ServerContextData.ServerName, session.NickName));

                await session.DisconnectAsync();
            }
            else
            {
                Logger.LogInformation("Valid password for session {SessionId}", session.SessionId);
            }
        }
    }
}
