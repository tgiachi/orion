using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Interfaces.Listeners.EventBus;

using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Types;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Network.Core.Extensions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Orion.Core.Server.Handlers.Base;

public abstract class BaseIrcCommandListener : IIrcCommandListener
{
    protected ILogger Logger { get; }

    protected IrcCommandListenerContext ListenerContext { get; }

    protected OrionServerConfig Config { get; }

    protected ITextTemplateService TextTemplateService { get; }


    protected string ServerHostName => ListenerContext.ServerContextData.ServerName;


    // Dictionary that maps command types and network types to their handlers
    private readonly
        Dictionary<(Type CommandType, ServerNetworkType NetworkType), Func<string, ServerNetworkType, IIrcCommand, Task>>
        _handlers = new();

    protected BaseIrcCommandListener(
        ILogger<BaseIrcCommandListener> logger, IrcCommandListenerContext context
    )
    {
        ListenerContext = context;
        Logger = logger;
        Config = context.AppContext.Config;
        TextTemplateService = context.TextTemplateService;
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

        ListenerContext.CommandService.AddListener<TCommand>(this, serverNetworkType);
    }

    protected List<IrcUserSession> QuerySessions(Func<IrcUserSession, bool> predicate)
    {
        // Query sessions based on a predicate
        return ListenerContext.SessionService.Sessions.Where(predicate).ToList();
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
                var session = ListenerContext.SessionService.GetSession(sessionId, false);
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
        ListenerContext.CommandService.AddListener<TCommand>(this, serverNetworkType);
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
        return ListenerContext.CommandService.SendCommandAsync<TCommand>(sessionId, command);
    }


    protected void SubscribeToEventBus<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        ListenerContext.EventBusService.Subscribe(listener);
    }

    protected Task PublishEventAsync<TEvent>(TEvent @event) where TEvent : class
    {
        return ListenerContext.EventBusService.PublishAsync(@event);
    }

    protected IrcUserSession? GetSession(string sessionId)
    {
        return ListenerContext.SessionService.GetSession(sessionId, false);
    }

    protected IrcUserSession? GetSessionByNickName(string nickName)
    {
        return ListenerContext.SessionService.FindByNickName(nickName);
    }

    protected string TranslateText(string text, object context = null)
    {
        return TextTemplateService.TranslateText(text, context);
    }
}
