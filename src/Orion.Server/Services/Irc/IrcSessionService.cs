using System.Collections.Concurrent;
using HyperCube.Postman.Interfaces.Services;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Foundations.Pool;

using Orion.Network.Core.Interfaces.Services;

namespace Orion.Server.Services.Irc;

public class IrcSessionService : IIrcSessionService
{
    private readonly ILogger _logger;

    private readonly ObjectPool<IrcUserSession> _sessionPool = new();

    private readonly ConcurrentDictionary<string, IrcUserSession> _sessions = new();

    private readonly INetworkTransportManager _networkTransportManager;
    private readonly IIrcCommandService _ircCommandService;

    private readonly IHyperPostmanService _hyperPostmanService;

    public IrcSessionService(
        ILogger<IrcSessionService> logger, INetworkTransportManager networkTransportManager,
        IIrcCommandService ircCommandService, IHyperPostmanService hyperPostmanService
    )
    {
        _logger = logger;
        _networkTransportManager = networkTransportManager;
        _ircCommandService = ircCommandService;
        _hyperPostmanService = hyperPostmanService;

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

        _hyperPostmanService.PublishAsync(new SessionDisconnectedEvent(sessionId));
    }

    private void OnClientConnected(string transportId, string sessionId, string endpoint)
    {
        var newSession = _sessionPool.Get();
        newSession.SessionId = sessionId;
        newSession.SetCommandService(_ircCommandService);

        _sessions.TryAdd(sessionId, newSession);
        var transport = _networkTransportManager.GetTransport(transportId);

        _logger.LogDebug(
            "Adding session {SessionId} to transport {TransportId} ({TransportType})",
            sessionId,
            transportId,
            transport.ServerNetworkType
        );

        _hyperPostmanService.PublishAsync(new SessionConnectedEvent(sessionId, endpoint, transport.ServerNetworkType));
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
}
