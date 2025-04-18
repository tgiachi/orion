using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Serilog;

namespace Orion.Server.Handlers;

public class PingPongHandler : BaseIrcCommandListener, IIrcCommandHandler<PingCommand>, IIrcCommandHandler<PongCommand>
{
    private readonly ISchedulerSystemService _schedulerSystemService;


    public PingPongHandler(
        ILogger<BaseIrcCommandListener> logger, IrcCommandListenerContext context,
        ISchedulerSystemService schedulerSystemService
    ) : base(logger, context)
    {
        _schedulerSystemService = schedulerSystemService;

        RegisterCommandHandler<PingCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<PongCommand>(this, ServerNetworkType.Clients);

        _schedulerSystemService.RegisterJob("ping_pong", PingPongJobTask, TimeSpan.FromSeconds(1));
        _schedulerSystemService.RegisterJob("registration_check", RegistrationCheckJobTask, TimeSpan.FromSeconds(1));
        _schedulerSystemService.RegisterJob("dead_ping", DisconnecteDeadPingTask, TimeSpan.FromSeconds(1));
    }

    private async Task DisconnecteDeadPingTask()
    {
        var sessionToDisconnected = QuerySessions(session =>
            session.LastPingResponse + TimeSpan.FromSeconds(10) <= DateTime.Now && session.IsAuthenticated
        );

        var rplError = ErrorCommand.CreateFromServer(ServerContextData.ServerName, "Ping timeout");

        foreach (var session in sessionToDisconnected)
        {
            await session.SendCommandAsync(rplError);
            await session.DisconnectAsync();
        }
    }

    private async Task RegistrationCheckJobTask()
    {
        var sessionToDisconnected = QuerySessions(session =>
            session.LastPingResponse + TimeSpan.FromSeconds(Config.Irc.Ping.Timeout) <= DateTime.Now &&
            !session.IsAuthenticated
        );

        var rplError = ErrorCommand.CreateFromServer(ServerContextData.ServerName, "Ping timeout");

        foreach (var session in sessionToDisconnected)
        {
            await session.SendCommandAsync(rplError);
            await session.DisconnectAsync();
        }
    }

    private async Task PingPongJobTask()
    {
        var sessionsToPing = QuerySessions(session =>
            session.LastPingResponse + TimeSpan.FromSeconds(Config.Irc.Ping.Interval) <= DateTime.Now
            && session.IsAuthenticated
        );

        var pingCommand = PingCommand.CreateFromServer(
            ServerContextData.ServerName,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        );

        foreach (var session in sessionsToPing)
        {
            await session.SendCommandAsync(pingCommand);
        }
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, PingCommand command
    )
    {
        session.SendCommandAsync(PongCommand.CreateFromServer(ServerContextData.ServerName, command.Token));
    }

    public Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, PongCommand command)
    {
        session.LastPingResponse = DateTime.Now;

        Logger.LogDebug("Received Pong from {SessionId}", session.SessionId);
        return Task.CompletedTask;
    }
}
