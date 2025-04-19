
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Interfaces.Listeners.EventBus;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Types;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Network.Core.Extensions;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Orion.Core.Server.Handlers.Base;

public abstract class BaseIrcCommandListener : IIrcCommandListener
{
    protected ILogger Logger { get; }

    private readonly IIrcCommandService _ircCommandService;

    private readonly IEventBusService _eventBusService;

    private readonly IIrcSessionService _sessionService;

    protected IrcServerContextData ServerContextData { get; }

    protected OrionServerConfig Config { get; }


    // Dictionary that maps command types and network types to their handlers
    private readonly
        Dictionary<(Type CommandType, ServerNetworkType NetworkType), Func<string, ServerNetworkType, IIrcCommand, Task>>
        _handlers = new();

    protected BaseIrcCommandListener(
        ILogger<BaseIrcCommandListener> logger, IrcCommandListenerContext context
    )
    {
        Logger = logger;
        _ircCommandService = context.CommandService;
        _eventBusService = context.EventBusService;
        _sessionService = context.SessionService;
        Config = context.AppContext.Config;
        ServerContextData = context.ServerContextData;
    }

    protected void RegisterHandler<TCommand>(
        Func<string, ServerNetworkType, TCommand, Task> handler, ServerNetworkType serverNetworkType
    )
        where TCommand : IIrcCommand, new()
    {
        // Register a handler for a specific command type and network type
        _handlers[(typeof(TCommand), serverNetworkType)] = (sessionId, networkType, command) =>
        {
            if (command is TCommand typedCommand)
            {
                return handler(sessionId, networkType, typedCommand);
            }

            return Task.CompletedTask;
        };

        _ircCommandService.AddListener<TCommand>(this, serverNetworkType);
    }

    protected List<IrcUserSession> QuerySessions(Func<IrcUserSession, bool> predicate)
    {
        // Query sessions based on a predicate
        return _sessionService.Sessions.Where(predicate).ToList();
    }

    protected void RegisterHandler<TCommand>(
        IIrcCommandListener<TCommand> listener, ServerNetworkType serverNetworkType
    )
        where TCommand : IIrcCommand, new()
    {
        RegisterHandler<TCommand>(listener.OnCommandReceivedAsync, serverNetworkType);
    }

    protected void RegisterCommandHandler<TCommand>(
        IIrcCommandHandler<TCommand> handler, ServerNetworkType serverNetworkType
    )
        where TCommand : IIrcCommand, new()
    {
        // Register a handler for a specific command type and network type
        _handlers[(typeof(TCommand), serverNetworkType)] = async (sessionId, networkType, command) =>
        {
            if (command is TCommand typedCommand)
            {
                var session = _sessionService.GetSession(sessionId, false);
                if (session != null)
                {
                    await handler.OnCommandReceivedAsync(session, networkType, typedCommand);
                }
                else
                {
                    Logger.LogWarning(
                        "Session {SessionId} not found for command {CommandType}",
                        sessionId.ToShortSessionId(),
                        typeof(TCommand).Name
                    );
                }
            }
        };

        var cmd = new TCommand();
        _ircCommandService.AddListener<TCommand>(this, serverNetworkType);
    }

    public virtual Task OnCommandReceivedAsync(string sessionId, ServerNetworkType serverNetworkType, IIrcCommand command)
    {
        Type commandType = command.GetType();

        // Now we have the serverNetworkType directly in the method parameters
        var key = (commandType, serverNetworkType);
        if (_handlers.TryGetValue(key, out var handler))
        {
            return handler(sessionId, serverNetworkType, command);
        }

        Logger.LogWarning(
            "No handler registered for command type {CommandType} on network type {NetworkType}",
            commandType.Name,
            serverNetworkType
        );
        return Task.CompletedTask;
    }

    protected Task SendCommandAsync<TCommand>(string sessionId, TCommand command) where TCommand : IIrcCommand
    {
        return _ircCommandService.SendCommandAsync<TCommand>(sessionId, command);
    }


    protected void SubscribeToPostman<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        _eventBusService.Subscribe(listener);
    }

    protected Task PublishEventAsync<TEvent>(TEvent @event) where TEvent : class
    {
        return _eventBusService.PublishAsync(@event);
    }

    protected IrcUserSession? GetSession(string sessionId)
    {
        return _sessionService.GetSession(sessionId, false);
    }
}
