using System.Text;
using Orion.Core.Irc.Server.Interfaces.Listeners.Commands;
using Orion.Core.Irc.Server.Interfaces.Services.Irc;
using Orion.Foundations.Observable;
using Orion.Foundations.Types;
using Orion.Irc.Core.Data.Internal;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Irc.Core.Interfaces.Parser;
using Orion.Network.Core.Data;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Parsers;
using Orion.Server.CommandListener;

namespace Orion.Server.Services.Irc;

public class IrcCommandService : IIrcCommandService, IDisposable
{
    private readonly ILogger _logger;

    private readonly IIrcCommandParser _ircCommandParser;

    private readonly IrcCommandListenerRegistry _commandListenerRegistry = new();

    private readonly INetworkTransportManager _networkTransportManager;

    private readonly ChannelObservable<NetworkMessageData> _channelListener;

    private readonly IDisposable _channelListenerDisposable;

    private readonly List<IrcCommandDefinitionData> _commands = new();

    public IrcCommandService(
        ILogger<IrcCommandService> logger, IIrcCommandParser ircCommandParser,
        INetworkTransportManager networkTransportManager, List<IrcCommandDefinitionData> commands = null
    )
    {
        _commands = commands ?? new List<IrcCommandDefinitionData>();
        _logger = logger;
        _ircCommandParser = ircCommandParser;
        _networkTransportManager = networkTransportManager;

        RegisterIrcCommands();


        _channelListener =
            new ChannelObservable<NetworkMessageData>(networkTransportManager.IncomingMessages);

        _channelListenerDisposable = _channelListener.Subscribe(async data =>
            await HandleIncomingMessageAsync(data.SessionId, data.Message, data.ServerNetworkType)
        );
    }

    private void RegisterIrcCommands()
    {
        foreach (var command in _commands)
        {
            _ircCommandParser.RegisterCommand(command.Command);
        }
    }


    private async Task HandleIncomingMessageAsync(
        string sessionId, byte[] buffer, ServerNetworkType serverNetworkType
    )
    {
        var messages = NewLineMessageParser.FastParseMessages(buffer);

        foreach (var message in messages)
        {
            var command = await _ircCommandParser.ParseAsync(message);

            var listeners = _commandListenerRegistry.GetListeners(serverNetworkType, command.Code);


            foreach (var listener in listeners)
            {
                try
                {
                    await listener.OnCommandReceivedAsync(sessionId, serverNetworkType, command);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while processing command {Command} for session {SessionId} in listener: {Listener}: {Message}",
                        command.Code,
                        sessionId,
                        listener.GetType().Name,
                        buffer
                    );
                }
            }
        }
    }

    public void AddListener<TCommand>(IIrcCommandListener listener, ServerNetworkType serverNetworkType)
        where TCommand : IIrcCommand, new()
    {
        var cmd = new TCommand();


        _logger.LogDebug("Adding listener {Listener} for command {Command}", listener.GetType().Name, cmd.Code);

        _commandListenerRegistry.Register<TCommand>(serverNetworkType, cmd.Code, listener);
    }

    public async Task SendCommandAsync<TCommand>(string sessionId, TCommand command) where TCommand : IIrcCommand
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var cmdString = await _ircCommandParser.SerializeAsync(command);

        var buffer = Encoding.UTF8.GetBytes(cmdString);

        await _networkTransportManager.EnqueueMessageAsync(
            new NetworkMessageData(sessionId, buffer, ServerNetworkType.None)
        );
    }

    public void Dispose()
    {
        _channelListenerDisposable?.Dispose();
    }
}
