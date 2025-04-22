using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Core.Server.Data.Sessions;

/// <summary>
/// Represents an active IRC user session on the server.
/// Manages user state, modes, and provides methods to interact with the connection.
/// </summary>
public class IrcUserSession : IDisposable, IEquatable<IrcUserSession>
{
    #region Fields

    /// <summary>
    /// Collection of active user modes for this session.
    /// </summary>
    private readonly HashSet<char> _userModes = new();

    /// <summary>
    /// Service for sending IRC commands to the client.
    /// </summary>
    private IIrcCommandService _ircCommandService;

    /// <summary>
    /// Network transport manager for network operations.
    /// </summary>
    private INetworkTransportManager _networkTransportManager;

    #endregion

    #region Properties - Connection Information

    /// <summary>
    /// Unique identifier for the session.
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// Remote endpoint in "address:port" format.
    /// </summary>
    public string Endpoint
    {
        get => $"{RemoteAddress}:{RemotePort}";
        set
        {
            var parts = value.Split(':');
            if (parts.Length == 2)
            {
                RemoteAddress = parts[0];
                RemotePort = int.Parse(parts[1]);
            }
            else
            {
                throw new ArgumentException("Invalid endpoint format. Expected 'address:port'.");
            }
        }
    }

    /// <summary>
    /// Remote IP address of the client.
    /// </summary>
    public string RemoteAddress { get; private set; }

    /// <summary>
    /// Remote port of the client.
    /// </summary>
    public int RemotePort { get; private set; }

    #endregion

    #region Properties - User Information

    /// <summary>
    /// IRC nickname of the user.
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// IRC username of the user.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Real name of the IRC user.
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// Hostname of the user, resolved from IP address.
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// Virtual hostname assigned to the user, if any.
    /// </summary>
    public string? VHostName { get; set; }

    /// <summary>
    /// Complete user address in "nick!user@host" format.
    /// </summary>
    public string FullAddress => $"{NickName}!{UserName}@{VHostName ?? HostName}";

    #endregion

    #region Properties - Session Status

    /// <summary>
    /// Date and time of the user's last activity.
    /// </summary>
    public DateTime LastActivity { get; set; }

    /// <summary>
    /// Date and time of the last PONG response received.
    /// </summary>
    public DateTime LastPingResponse { get; set; }

    /// <summary>
    /// Indicates if the USER command has been sent by the client.
    /// </summary>
    public bool IsUserSent => !string.IsNullOrEmpty(UserName);

    /// <summary>
    /// Indicates if the NICK command has been sent by the client.
    /// </summary>
    public bool IsNickSent => !string.IsNullOrEmpty(NickName);

    /// <summary>
    /// Indicates if the password provided by the client is valid.
    /// </summary>
    public bool IsPasswordValid { get; set; }

    /// <summary>
    /// Indicates if the user is fully authenticated.
    /// </summary>
    public bool IsAuthenticated => IsUserSent && IsNickSent && !IsRegistered;

    public bool IsRegistered { get; set; }

    /// <summary>
    /// Indicates if the user has set AWAY status.
    /// </summary>
    public bool IsAway { get; private set; }

    /// <summary>
    /// The user's AWAY message, if any.
    /// </summary>
    public string AwayMessage { get; private set; }

    #endregion

    #region Properties - User Modes

    /// <summary>
    /// Gets the user modes as a string.
    /// </summary>
    public string ModesString => new(_userModes.ToArray());

    /// <summary>
    /// Indicates if the user is invisible (mode +i).
    /// </summary>
    public bool IsInvisible => HasMode('i');

    /// <summary>
    /// Indicates if the user is an IRC operator (mode +o).
    /// </summary>
    public bool IsOperator => HasMode('o');

    /// <summary>
    /// Indicates if the user receives WALLOPS messages (mode +w).
    /// </summary>
    public bool ReceivesWallops => HasMode('w');

    /// <summary>
    /// Indicates if the user is registered with services (mode +r).
    /// </summary>
    public bool IsRegisteredUser => HasMode('r');


    public void SetOperator(bool isOperator)
    {
        if (isOperator)
        {
            AddMode('o');
        }
        else
        {
            RemoveMode('o');
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IrcUserSession"/> class.
    /// </summary>
    public IrcUserSession()
    {
        Initialize();
    }

    #endregion

    #region Public Methods - Dependency Injection

    /// <summary>
    /// Sets the IRC command service for this session.
    /// </summary>
    /// <param name="ircCommandService">The IRC command service.</param>
    public void SetCommandService(IIrcCommandService ircCommandService)
    {
        _ircCommandService = ircCommandService;
    }

    /// <summary>
    /// Sets the network transport manager for this session.
    /// </summary>
    /// <param name="networkTransportManager">The network transport manager.</param>
    public void SetNetworkTransportManager(INetworkTransportManager networkTransportManager)
    {
        _networkTransportManager = networkTransportManager;
    }

    #endregion

    #region Public Methods - Communication

    /// <summary>
    /// Sends one or more IRC commands to the client.
    /// </summary>
    /// <param name="commands">The commands to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendCommandAsync(params IIrcCommand[] commands)
    {
        foreach (var command in commands)
        {
            await _ircCommandService.SendCommandAsync(SessionId, command);
        }
    }

    /// <summary>
    /// Disconnects the client from the server.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DisconnectAsync()
    {
        await _networkTransportManager.DisconnectAsync(SessionId);
    }

    #endregion

    #region Public Methods - User Modes

    /// <summary>
    /// Checks if the user has a specific mode.
    /// </summary>
    /// <param name="mode">The character representing the mode.</param>
    /// <returns>True if the user has the specified mode, otherwise false.</returns>
    public bool HasMode(char mode)
    {
        return _userModes.Contains(mode);
    }

    /// <summary>
    /// Adds a mode to the user.
    /// </summary>
    /// <param name="mode">The character representing the mode to add.</param>
    /// <returns>True if the mode was added, false if it was already present.</returns>
    public bool AddMode(char mode)
    {
        return _userModes.Add(mode);
    }

    /// <summary>
    /// Removes a mode from the user.
    /// </summary>
    /// <param name="mode">The character representing the mode to remove.</param>
    /// <returns>True if the mode was removed, false if it wasn't present.</returns>
    public bool RemoveMode(char mode)
    {
        return _userModes.Remove(mode);
    }

    /// <summary>
    /// Applies a mode change string to the user.
    /// </summary>
    /// <param name="modeString">The mode string to apply (e.g., "+iw-o").</param>
    public void ApplyModeChanges(string modeString)
    {
        if (string.IsNullOrEmpty(modeString))
        {
            return;
        }

        char action = '+';

        foreach (char c in modeString)
        {
            if (c == '+' || c == '-')
            {
                action = c;
            }
            else
            {
                if (action == '+')
                {
                    AddMode(c);
                }
                else if (action == '-')
                {
                    RemoveMode(c);
                }
            }
        }
    }

    #endregion

    #region Public Methods - Away Status

    /// <summary>
    /// Sets the user as AWAY with a specific message.
    /// </summary>
    /// <param name="message">The away message.</param>
    public void SetAway(string message)
    {
        IsAway = true;
        AwayMessage = message;
    }

    /// <summary>
    /// Removes the user's AWAY status.
    /// </summary>
    public void SetBack()
    {
        IsAway = false;
        AwayMessage = null;
    }

    #endregion

    #region Public Methods - Initialization

    /// <summary>
    /// Initializes or resets the session state to default values.
    /// </summary>
    public void Initialize()
    {
        LastActivity = DateTime.Now;
        LastPingResponse = DateTime.Now;
        RemoteAddress = string.Empty;
        RemotePort = 0;
        HostName = string.Empty;
        VHostName = null;
        UserName = string.Empty;
        RealName = string.Empty;
        NickName = string.Empty;
        IsPasswordValid = false;

        IsAway = false;
        AwayMessage = string.Empty;
        _userModes.Clear();
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Releases resources used by the session.
    /// </summary>
    public void Dispose()
    {
        // Implement resource cleanup if needed
        GC.SuppressFinalize(this);
    }

    #endregion

    public bool Equals(IrcUserSession? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return SessionId == other.SessionId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((IrcUserSession)obj);
    }

    public override int GetHashCode()
    {
        return SessionId.GetHashCode();
    }
}
