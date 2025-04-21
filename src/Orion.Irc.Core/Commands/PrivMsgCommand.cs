using System;
using System.Collections.Generic;
using System.Linq;
using Orion.Irc.Core.Commands.Base;
using Orion.Irc.Core.Data.Messages;

namespace Orion.Irc.Core.Commands;

/// <summary>
/// Represents an IRC PRIVMSG command used for sending messages to users or channels
/// </summary>
public class PrivMsgCommand : BaseIrcCommand
{
    private string _target;
    private PrivMessageTarget _targetObject;

    /// <summary>
    /// The source of the message (user or server)
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// The raw target of the message (user nickname, channel name, or comma-separated list)
    /// </summary>
    public string Target
    {
        get => _target;
        set
        {
            _target = value;
            _targetObject = value != null ? new PrivMessageTarget(value) : null;
        }
    }

    /// <summary>
    /// The target object providing typed information about the message recipient
    /// </summary>
    public PrivMessageTarget TargetObject => _targetObject;

    /// <summary>
    /// Multiple targets if the target string contains comma-separated values
    /// </summary>
    public PrivMessageTarget[] Targets => PrivMessageTarget.ParseTargets(Target);

    /// <summary>
    /// The message content
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Indicates if the message is a CTCP request/response
    /// </summary>
    public bool IsCtcp => Message?.StartsWith('\u0001') == true && Message?.EndsWith('\u0001') == true;

    /// <summary>
    /// Checks if the target is a channel message
    /// </summary>
    public bool IsChannelMessage => TargetObject?.IsChannel == true;

    /// <summary>
    /// Checks if the target is a user message
    /// </summary>
    public bool IsUserMessage => TargetObject?.IsUser == true;

    /// <summary>
    /// Checks if the message targets multiple recipients
    /// </summary>
    public bool HasMultipleTargets => Target?.Contains(',') == true;

    /// <summary>
    /// Gets the CTCP command if this is a CTCP message
    /// </summary>
    public string CtcpCommand
    {
        get
        {
            if (!IsCtcp)
            {
                return null;
            }

            // Remove the \u0001 at start and end
            string content = Message.Substring(1, Message.Length - 2);
            int spacePos = content.IndexOf(' ');

            return spacePos > 0 ? content[..spacePos] : content;
        }
    }

    /// <summary>
    /// Gets the CTCP parameters if this is a CTCP message
    /// </summary>
    public string CtcpParameters
    {
        get
        {
            if (!IsCtcp)
            {
                return null;
            }

            // Remove the \u0001 at start and end
            string content = Message.Substring(1, Message.Length - 2);
            int spacePos = content.IndexOf(' ');

            return spacePos > 0 ? content[(spacePos + 1)..] : string.Empty;
        }
    }

    /// <summary>
    /// Creates a new empty PRIVMSG command
    /// </summary>
    public PrivMsgCommand() : base("PRIVMSG")
    {
    }

    /// <summary>
    /// Creates a new PRIVMSG command with the specified parameters
    /// </summary>
    /// <param name="source">Source of the message</param>
    /// <param name="target">Target of the message</param>
    /// <param name="message">Message content</param>
    public PrivMsgCommand(string source, string target, string message) : base("PRIVMSG")
    {
        Source = source;
        Target = target;
        Message = message;
    }

    /// <summary>
    /// Creates a new PRIVMSG command with a typed target
    /// </summary>
    /// <param name="source">Source of the message</param>
    /// <param name="target">Target object for the message</param>
    /// <param name="message">Message content</param>
    public PrivMsgCommand(string source, PrivMessageTarget target, string message) : base("PRIVMSG")
    {
        Source = source;
        Target = target?.ToString();
        _targetObject = target;
        Message = message;
    }

    /// <summary>
    /// Parses a PRIVMSG command from a raw IRC message
    /// </summary>
    /// <param name="line">The raw IRC message</param>
    public override void Parse(string line)
    {
        // Reset existing data
        Source = null;
        Target = null;
        Message = null;

        // Check if line starts with a source prefix
        if (line.StartsWith(':'))
        {
            // Find the first space after the source
            int sourceEndIndex = line.IndexOf(' ');
            if (sourceEndIndex != -1)
            {
                Source = line.Substring(1, sourceEndIndex - 1);
                line = line.Substring(sourceEndIndex).TrimStart();
            }
        }

        // Tokenize remaining line
        var tokens = line.Split(new[] { ' ' }, 3);

        // Validate tokens
        if (tokens.Length < 3 || tokens[0].ToUpper() != "PRIVMSG")
        {
            throw new FormatException($"Invalid PRIVMSG format: {line}");
        }

        // Set target
        Target = tokens[1];

        // Extract message (remove leading : if present)
        Message = tokens[2].StartsWith(":")
            ? tokens[2].Substring(1)
            : tokens[2];
    }

    /// <summary>
    /// Converts the command to its string representation
    /// </summary>
    /// <returns>The IRC protocol representation of the command</returns>
    public override string Write()
    {
        if (!string.IsNullOrEmpty(Source))
        {
            return $":{Source} PRIVMSG {Target} :{Message}";
        }
        else
        {
            return $"PRIVMSG {Target} :{Message}";
        }
    }

    #region Factory Methods

    /// <summary>
    /// Creates a PRIVMSG from a user to a target
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="target">Target (channel or user)</param>
    /// <param name="message">Message content</param>
    /// <returns>A new PrivMessageCommand</returns>
    public static PrivMsgCommand CreateFromUser(string userPrefix, string target, string message)
    {
        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = target,
            Message = message
        };
    }

    /// <summary>
    /// Creates a PRIVMSG from a user to a typed target
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="target">Target object</param>
    /// <param name="message">Message content</param>
    /// <returns>A new PrivMessageCommand</returns>
    public static PrivMsgCommand CreateFromUser(string userPrefix, PrivMessageTarget target, string message)
    {
        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = target?.ToString(),
            _targetObject = target,
            Message = message
        };
    }

    /// <summary>
    /// Creates a CTCP message (Client-To-Client Protocol)
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="target">Target (channel or user)</param>
    /// <param name="ctcpCommand">CTCP command</param>
    /// <param name="parameters">Optional CTCP parameters</param>
    /// <returns>A new PrivMessageCommand for the CTCP message</returns>
    public static PrivMsgCommand CreateCtcp(string userPrefix, string target, string ctcpCommand, string parameters = null)
    {
        string ctcpMessage = string.IsNullOrEmpty(parameters)
            ? $"\u0001{ctcpCommand}\u0001"
            : $"\u0001{ctcpCommand} {parameters}\u0001";

        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = target,
            Message = ctcpMessage
        };
    }

    /// <summary>
    /// Creates a CTCP message with a typed target
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="target">Target object</param>
    /// <param name="ctcpCommand">CTCP command</param>
    /// <param name="parameters">Optional CTCP parameters</param>
    /// <returns>A new PrivMessageCommand for the CTCP message</returns>
    public static PrivMsgCommand CreateCtcp(string userPrefix, PrivMessageTarget target, string ctcpCommand, string parameters = null)
    {
        string ctcpMessage = string.IsNullOrEmpty(parameters)
            ? $"\u0001{ctcpCommand}\u0001"
            : $"\u0001{ctcpCommand} {parameters}\u0001";

        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = target?.ToString(),
            _targetObject = target,
            Message = ctcpMessage
        };
    }

    /// <summary>
    /// Creates an ACTION message (special CTCP message for describing actions)
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="target">Target (channel or user)</param>
    /// <param name="action">Action description</param>
    /// <returns>A new PrivMessageCommand for the action</returns>
    public static PrivMsgCommand CreateAction(string userPrefix, string target, string action)
    {
        return CreateCtcp(userPrefix, target, "ACTION", action);
    }

    /// <summary>
    /// Creates an ACTION message with a typed target
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="target">Target object</param>
    /// <param name="action">Action description</param>
    /// <returns>A new PrivMessageCommand for the action</returns>
    public static PrivMsgCommand CreateAction(string userPrefix, PrivMessageTarget target, string action)
    {
        return CreateCtcp(userPrefix, target, "ACTION", action);
    }

    /// <summary>
    /// Creates a PRIVMSG to a channel
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="channel">Channel name</param>
    /// <param name="message">Message content</param>
    /// <returns>A new PrivMessageCommand for the channel message</returns>
    public static PrivMsgCommand CreateToChannel(string userPrefix, string channel, string message)
    {
        var channelTarget = PrivMessageTarget.CreateChannelTarget(channel);
        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = channelTarget.ToString(),
            _targetObject = channelTarget,
            Message = message
        };
    }

    /// <summary>
    /// Creates a PRIVMSG to a user
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="nickname">Target user's nickname</param>
    /// <param name="message">Message content</param>
    /// <returns>A new PrivMessageCommand for the user message</returns>
    public static PrivMsgCommand CreateToUser(string userPrefix, string nickname, string message)
    {
        var userTarget = PrivMessageTarget.CreateUserTarget(nickname);
        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = userTarget.ToString(),
            _targetObject = userTarget,
            Message = message
        };
    }

    /// <summary>
    /// Creates a PRIVMSG to multiple targets
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="targets">Collection of targets</param>
    /// <param name="message">Message content</param>
    /// <returns>A new PrivMessageCommand for the multi-target message</returns>
    public static PrivMsgCommand CreateToMultipleTargets(string userPrefix, IEnumerable<string> targets, string message)
    {
        string targetsStr = string.Join(",", targets);
        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = targetsStr,
            Message = message
        };
    }

    /// <summary>
    /// Creates a PRIVMSG to multiple targets
    /// </summary>
    /// <param name="userPrefix">Source user's prefix (nick!user@host)</param>
    /// <param name="targets">Collection of target objects</param>
    /// <param name="message">Message content</param>
    /// <returns>A new PrivMessageCommand for the multi-target message</returns>
    public static PrivMsgCommand CreateToMultipleTargets(string userPrefix, IEnumerable<PrivMessageTarget> targets, string message)
    {
        string targetsStr = string.Join(",", targets.Select(t => t.ToString()));
        return new PrivMsgCommand
        {
            Source = userPrefix,
            Target = targetsStr,
            Message = message
        };
    }

    #endregion
}
