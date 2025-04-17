using Microsoft.Extensions.Logging;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Core.Types;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Server.Handlers.Base;

public class BaseIrcCommandListener : IIrcCommandListener
{
    private readonly ILogger _logger;

    private readonly IIrcCommandService _ircCommandService;


    // Dictionary that maps command types and network types to their handlers
    private readonly Dictionary<(Type CommandType, ServerNetworkType NetworkType), Func<string, ServerNetworkType, IIrcCommand, Task>> _handlers = new();

    public BaseIrcCommandListener(ILogger<BaseIrcCommandListener> logger, IIrcCommandService ircCommandService)
    {
        _logger = logger;
        _ircCommandService = ircCommandService;
    }

    protected void RegisterHandler<TCommand>(Func<string, ServerNetworkType, TCommand, Task> handler, ServerNetworkType serverNetworkType)
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

        _logger.LogWarning("No handler registered for command type {CommandType} on network type {NetworkType}",
            commandType.Name, serverNetworkType);
        return Task.CompletedTask;
    }
}
