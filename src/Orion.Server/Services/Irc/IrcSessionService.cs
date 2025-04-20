
using System.Collections.Concurrent;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Pool;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Server.Services.Irc;

public class IrcSessionService : IIrcSessionService
{

    public int TotalSessions => _sessions.Count;
    public int TotalInvisibleSessions => _sessions.Values.Count(x => x.IsInvisible);
    public int MaxSessions { get; private set; }
    public int TotalOpers => _sessions.Values.Count(x => x.IsOperator);

    private readonly ILogger _logger;

    private readonly ObjectPool<IrcUserSession> _sessionPool = new();


    private readonly ConcurrentDictionary<string, IrcUserSession> _sessions = new();

    private readonly INetworkTransportManager _networkTransportManager;
    private readonly IIrcCommandService _ircCommandService;

    private readonly IEventBusService _eventBusService;

    public List<IrcUserSession> Sessions => _sessions.Values.ToList();

    public IrcSessionService(
        ILogger<IrcSessionService> logger, INetworkTransportManager networkTransportManager,
        IIrcCommandService ircCommandService, IEventBusService eventBusService
    )
    {
        _logger = logger;
        _networkTransportManager = networkTransportManager;
        _ircCommandService = ircCommandService;
        _eventBusService = eventBusService;

        _networkTransportManager.ClientConnected += OnClientConnected;
        _networkTransportManager.ClientDisconnected += OnClientDisconnected;
    }

    private void OnClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        if (_sessions.TryRemove(sessionId, out var session))
        {
            _logger.LogDebug("Removing session {SessionId}", sessionId);
            session.Dispose();
            _sessionPool.Return(session);
        }

        _eventBusService.PublishAsync(new SessionDisconnectedEvent(sessionId));
    }

    private void OnClientConnected(string transportId, string sessionId, string endpoint)
    {
        var newSession = _sessionPool.Get();
        newSession.Initialize();

        newSession.Endpoint = endpoint;
        newSession.SessionId = sessionId;
        newSession.SetCommandService(_ircCommandService);
        newSession.SetNetworkTransportManager(_networkTransportManager);

        _sessions.TryAdd(sessionId, newSession);
        var transport = _networkTransportManager.GetTransport(transportId);

        _logger.LogDebug(
            "Adding session {SessionId} to transport {TransportId} ({TransportType})",
            sessionId,
            transportId,
            transport.ServerNetworkType
        );

        _eventBusService.PublishAsync(new SessionConnectedEvent(sessionId, endpoint, transport.ServerNetworkType));

        MaxSessions++;
    }


    public IrcUserSession GetSession(string sessionId, bool throwIfNotFound = true)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            return session;
        }

        if (throwIfNotFound)
        {
            throw new KeyNotFoundException($"Session '{sessionId}' not found");
        }

        return null;
    }

    public List<IrcUserSession> QuerySessions(Func<IrcUserSession, bool> predicate)
    {
        return Sessions.Where(predicate).ToList();
    }


}
