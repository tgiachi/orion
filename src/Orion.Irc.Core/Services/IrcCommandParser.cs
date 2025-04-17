using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Irc.Core.Interfaces.Parser;

namespace Orion.Irc.Core.Services;

public class IrcCommandParser : IIrcCommandParser
{
    private readonly ILogger _logger;

    private readonly Dictionary<string, IIrcCommand> _commands = new();

    public IrcCommandParser(ILogger<IrcCommandParser> logger)
    {
        _logger = logger;
    }

    public async Task<IIrcCommand> ParseAsync(string message)
    {
        var sw = Stopwatch.GetTimestamp();

        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Received empty message");
                return new NotParsedCommand();
            }

            _logger.LogDebug("Parsing line: {Line}", message);

            // Split the command and parameters
            string[] parts = message.Split(' ');
            string command = parts[0].ToUpperInvariant();

            // Create the appropriate command object based on the command string
            _commands.TryGetValue(command, out var ircCommand);

            if (ircCommand != null)
            {
                try
                {
                    ircCommand.Parse(message);
                    _logger.LogDebug("Parsed command: {CommandType}", ircCommand.GetType().Name);


                    _logger.LogDebug("Parsed command in {Elapsed}ms", Stopwatch.GetElapsedTime(sw));
                    return ircCommand;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing command {Command}: {Line}", command, message);
                }
            }
            else
            {
                _logger.LogWarning("Unknown command {Command}: {Line}", command, message);
                var notParsed = new NotParsedCommand();
                notParsed.Parse(message);
                return notParsed;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse message {Message}", message);
        }

        throw new InvalidOperationException("Failed to parse message");
    }

    public async Task<string> SerializeAsync(IIrcCommand command)
    {
        return command.Write();
    }

    public void RegisterCommand<TCommand>() where TCommand : IIrcCommand, new()
    {
        var command = new TCommand();

        RegisterCommand(command);
    }

    public void RegisterCommand(IIrcCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        _commands[command.Code] = command;

        _logger.LogDebug("Registered IRC command {CommandName}", command.Code);
    }
}
