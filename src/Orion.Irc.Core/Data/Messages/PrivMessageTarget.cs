namespace Orion.Irc.Core.Data.Messages;

/// <summary>
/// Represents a target for a PRIVMSG command with information about the target type
/// </summary>
public class PrivMessageTarget
{
    /// <summary>
    /// The type of the target
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// Target is a user (nickname)
        /// </summary>
        User,

        /// <summary>
        /// Target is a channel
        /// </summary>
        Channel,

        /// <summary>
        /// Target is a server mask
        /// </summary>
        ServerMask,

        /// <summary>
        /// Target is unknown or invalid
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Gets the raw target string
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Gets the type of the target
    /// </summary>
    public TargetType Type { get; }

    /// <summary>
    /// Gets whether the target is a channel
    /// </summary>
    public bool IsChannel => Type == TargetType.Channel;

    /// <summary>
    /// Gets whether the target is a user
    /// </summary>
    public bool IsUser => Type == TargetType.User;

    /// <summary>
    /// Gets whether the target is a server mask
    /// </summary>
    public bool IsServerMask => Type == TargetType.ServerMask;

    /// <summary>
    /// Creates a new PrivMessageTarget for the specified target string
    /// </summary>
    /// <param name="target">The target string</param>
    public PrivMessageTarget(string target)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Type = DetermineTargetType(target);
    }

    /// <summary>
    /// Determines the type of the target based on the target string format
    /// </summary>
    /// <param name="target">The target string to analyze</param>
    /// <returns>The determined target type</returns>
    private static TargetType DetermineTargetType(string target)
    {
        if (string.IsNullOrEmpty(target))
            return TargetType.Unknown;

        // Channels start with #, &, +, or !
        if (target.StartsWith('#') || target.StartsWith('&') ||
            target.StartsWith('+') || target.StartsWith('!'))
            return TargetType.Channel;

        // Server masks contain wildcards (* or ?) or have a specific format
        if (target.Contains('*') || target.Contains('?') || target.Contains('.'))
            return TargetType.ServerMask;

        // Otherwise, assume it's a user
        return TargetType.User;
    }

    /// <summary>
    /// Creates a channel target with the specified name
    /// </summary>
    /// <param name="channelName">The channel name</param>
    /// <returns>A new PrivMessageTarget for the channel</returns>
    public static PrivMessageTarget CreateChannelTarget(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
            throw new ArgumentException("Channel name cannot be empty", nameof(channelName));

        // Ensure channel name starts with appropriate prefix
        if (!channelName.StartsWith('#') && !channelName.StartsWith('&') &&
            !channelName.StartsWith('+') && !channelName.StartsWith('!'))
            channelName = '#' + channelName;

        return new PrivMessageTarget(channelName);
    }

    /// <summary>
    /// Creates a user target with the specified nickname
    /// </summary>
    /// <param name="nickname">The user's nickname</param>
    /// <returns>A new PrivMessageTarget for the user</returns>
    public static PrivMessageTarget CreateUserTarget(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
            throw new ArgumentException("Nickname cannot be empty", nameof(nickname));

        return new PrivMessageTarget(nickname);
    }

    /// <summary>
    /// Creates a server mask target
    /// </summary>
    /// <param name="mask">The server mask</param>
    /// <returns>A new PrivMessageTarget for the server mask</returns>
    public static PrivMessageTarget CreateServerMaskTarget(string mask)
    {
        if (string.IsNullOrEmpty(mask))
            throw new ArgumentException("Server mask cannot be empty", nameof(mask));

        return new PrivMessageTarget(mask);
    }

    /// <summary>
    /// Parse multiple targets from a comma-separated list
    /// </summary>
    /// <param name="targetList">Comma-separated list of targets</param>
    /// <returns>Array of PrivMessageTarget objects</returns>
    public static PrivMessageTarget[] ParseTargets(string targetList)
    {
        if (string.IsNullOrEmpty(targetList))
            return Array.Empty<PrivMessageTarget>();

        var targets = targetList.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var result = new PrivMessageTarget[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            result[i] = new PrivMessageTarget(targets[i].Trim());
        }

        return result;
    }

    /// <summary>
    /// Returns the raw target string
    /// </summary>
    public override string ToString() => Target;

    /// <summary>
    /// Implicitly converts a PrivMessageTarget to its string representation
    /// </summary>
    public static implicit operator string(PrivMessageTarget target) => target?.Target;
}
