using System.Reactive.Linq;
using System.Reactive.Subjects;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services;
using Orion.Core.Types;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Irc.Core.Interfaces.Parser;
using Orion.Network.Core.Data;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Server.Services;

public class IrcCommandService : IIrcCommandService, IDisposable
{
    private readonly ILogger _logger;

    private readonly IIrcCommandParser _ircCommandParser;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly Dictionary<ServerNetworkType, Dictionary<string, List<IIrcCommandListener<IIrcCommand>>>>
        _commandListeners = new();

    private readonly INetworkTransportManager _networkTransportManager;

    public IrcCommandService(
        ILogger logger, IIrcCommandParser ircCommandParser, INetworkTransportManager networkTransportManager
    )
    {
        _logger = logger;
        _ircCommandParser = ircCommandParser;
        _networkTransportManager = networkTransportManager;

        foreach (ServerNetworkType serverNetworkType in Enum.GetValues<ServerNetworkType>())
        {
            _commandListeners[serverNetworkType] = new Dictionary<string, List<IIrcCommandListener<IIrcCommand>>>();
        }

        _ = Task.Run(ReadIncomingMessagesTask);
    }

    private async Task ReadIncomingMessagesTask()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            await foreach (var messageData in _networkTransportManager.IncomingMessages.Reader.ReadAllAsync(
                               _cancellationTokenSource.Token))
            {
                string sessionId = messageData.SessionId;
                string message = messageData.Message;
                var serverNetworkType = messageData.ServerNetworkType;

                try
                {
                    await HandleIncomingMessageAsync(sessionId, message, serverNetworkType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling incoming message: {Message}", message);
                }
            }
        }
    }

    private async Task HandleIncomingMessageAsync(
        string sessionId, string message, ServerNetworkType serverNetworkType
    )
    {
        IIrcCommand command = await _ircCommandParser.ParseAsync(message);

        if (_commandListeners[serverNetworkType].TryGetValue(command.Code, out var listeners))
        {
            foreach (var listener in listeners)
            {
                await listener.OnCommandReceivedAsync(sessionId, command);
            }
        }
    }

    public void AddListener<TCommand>(IIrcCommandListener<TCommand> listener, ServerNetworkType serverNetworkType)
        where TCommand : IIrcCommand, new()
    {
        var cmd = new TCommand();
        if (!_commandListeners[serverNetworkType].TryGetValue(cmd.Code, out List<IIrcCommandListener<IIrcCommand>>? value))
        {
            value = new List<IIrcCommandListener<IIrcCommand>>();
            _commandListeners[serverNetworkType][cmd.Code] = value;
        }

        value.Add((IIrcCommandListener<IIrcCommand>)listener);
    }

    public async Task SendCommandAsync<TCommand>(string sessionId, TCommand command) where TCommand : IIrcCommand
    {
        await _networkTransportManager.EnqueueMessageAsync(
            new NetworkMessageData(sessionId, await _ircCommandParser.SerializeAsync(command), ServerNetworkType.Clients)
        );
    }

    public void Dispose()
    {
    }
}
