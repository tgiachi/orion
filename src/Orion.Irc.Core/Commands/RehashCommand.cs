using Orion.Irc.Core.Commands.Base;

namespace Orion.Irc.Core.Commands;

/// <summary>
/// Represents an IRC REHASH command used to reload server configuration.
/// This command is typically restricted to IRC operators and administrators.
/// </summary>
public class RehashCommand : BaseIrcCommand
{
    /// <summary>
    /// Gets or sets the source/prefix of the command (if sent through a server).
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the target server (optional, used in server-to-server communication).
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// Gets or sets the specific configuration component to rehash (if server supports it).
    /// For example, "tls", "motd", "dns", etc.
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RehashCommand"/> class.
    /// </summary>
    public RehashCommand() : base("REHASH")
    {
    }

    /// <summary>
    /// Parses a raw IRC message line.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    public override void Parse(string line)
    {
        // Reset properties
        Source = null;
        Target = null;
        Component = null;

        // Check for source prefix
        if (line.StartsWith(':'))
        {
            int spaceIndex = line.IndexOf(' ');
            if (spaceIndex > 0)
            {
                Source = line.Substring(1, spaceIndex - 1);
                line = line.Substring(spaceIndex + 1);
            }
        }

        // Split the remaining parts
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // First part should be "REHASH"
        if (parts.Length == 0 || !string.Equals(parts[0], "REHASH", StringComparison.OrdinalIgnoreCase))
        {
            return; // Invalid format
        }

        // Check for target server
        if (parts.Length > 1)
        {
            Target = parts[1];
        }

        // Check for optional component
        if (parts.Length > 2)
        {
            Component = parts[2];
        }
    }

    /// <summary>
    /// Converts the command to its string representation.
    /// </summary>
    /// <returns>The formatted REHASH command string.</returns>
    public override string Write()
    {
        var result = "";

        // Add source prefix if present
        if (!string.IsNullOrEmpty(Source))
        {
            result += ":" + Source + " ";
        }

        // Add the command
        result += "REHASH";

        // Add target server if specified
        if (!string.IsNullOrEmpty(Target))
        {
            result += " " + Target;
        }

        // Add component if specified
        if (!string.IsNullOrEmpty(Component))
        {
            result += " " + Component;
        }

        return result;
    }

    /// <summary>
    /// Creates a basic REHASH command to reload the entire server configuration.
    /// </summary>
    /// <returns>A new RehashCommand instance.</returns>
    public static RehashCommand Create()
    {
        return new RehashCommand();
    }

    /// <summary>
    /// Creates a REHASH command targeting a specific server.
    /// </summary>
    /// <param name="target">The target server name.</param>
    /// <returns>A new RehashCommand instance.</returns>
    public static RehashCommand Create(string target)
    {
        return new RehashCommand
        {
            Target = target
        };
    }

    /// <summary>
    /// Creates a REHASH command for a specific configuration component.
    /// </summary>
    /// <param name="component">The configuration component to rehash (e.g., "tls", "motd").</param>
    /// <returns>A new RehashCommand instance.</returns>
    public static RehashCommand CreateForComponent(string component)
    {
        return new RehashCommand
        {
            Component = component
        };
    }

    /// <summary>
    /// Creates a REHASH command targeting a specific server and component.
    /// </summary>
    /// <param name="target">The target server name.</param>
    /// <param name="component">The configuration component to rehash.</param>
    /// <returns>A new RehashCommand instance.</returns>
    public static RehashCommand Create(string target, string component)
    {
        return new RehashCommand
        {
            Target = target,
            Component = component
        };
    }
}
