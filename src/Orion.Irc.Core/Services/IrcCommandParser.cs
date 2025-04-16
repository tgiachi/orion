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

    public async Task<List<IIrcCommand>> ParseAsync(string message)
    {
        var sw = Stopwatch.StartNew();
        var commands = new List<IIrcCommand>();
        try
        {
            foreach (var line in SanitizeMessage(message))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                _logger.LogDebug("Parsing line: {Line}", line);

                // Split the command and parameters
                string[] parts = line.Split(' ');
                string command = parts[0].ToUpperInvariant();

                // Create the appropriate command object based on the command string
                _commands.TryGetValue(command, out var ircCommand);

                if (ircCommand != null)
                {
                    try
                    {
                        ircCommand.Parse(line);
                        _logger.LogDebug("Parsed command: {CommandType}", ircCommand.GetType().Name);
                        commands.Add(ircCommand);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error parsing command {Command}: {Line}", command, line);
                    }
                }
                else
                {
                    _logger.LogWarning("Unknown command {Command}: {Line}", command, line);
                    var notParsed = new NotParsedCommand();
                    notParsed.Parse(line);
                    commands.Add(notParsed);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse message {Message}", message);
        }
        finally
        {
            sw.Stop();
            _logger.LogDebug("Parsed {CommandCount} commands in {Elapsed}ms", commands.Count, sw.ElapsedMilliseconds);
        }

        return commands;
    }

    public async Task<string> SerializeAsync(IIrcCommand command)
    {
        return command.Write();
    }

    public void RegisterCommand(IIrcCommand command)
    {
        _commands[command.Code] = command;

        _logger.LogDebug("Registered command {CommandName}", command.Code);
    }

    public List<string> SanitizeMessage(string rawMessage)
    {
        var lines = new Regex(@"\r\n|\n").Split(rawMessage);

        return lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
    }
}
