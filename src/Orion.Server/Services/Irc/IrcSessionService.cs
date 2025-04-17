using System.Collections.Concurrent;
using Orion.Core.Pool;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Server.Services.Irc;

public class IrcSessionService : IIrcSessionService
{
    private readonly ILogger _logger;

    private readonly ObjectPool<IrcUserSession> _sessionPool = new();

    private readonly ConcurrentDictionary<string, IrcUserSession> _sessions = new();

    private readonly INetworkTransportManager _networkTransportManager;
    private readonly IIrcCommandService _ircCommandService;

    public IrcSessionService(
        ILogger<IrcSessionService> logger, INetworkTransportManager networkTransportManager,
        IIrcCommandService ircCommandService
    )
    {
        _logger = logger;
        _networkTransportManager = networkTransportManager;
        _ircCommandService = ircCommandService;

        _networkTransportManager.ClientConnected += OnClientConnected;
        _networkTransportManager.ClientDisconnected += OnClientDisconnected;
    }

    private void OnClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        if (_sessions.TryRemove(sessionId, out var session))
        {
            session.Dispose();
            _sessionPool.Return(session);
        }
    }

    private void OnClientConnected(string transportId, string sessionId, string endpoint)
    {
        var newSession = _sessionPool.Get();
        newSession.SessionId = sessionId;
        newSession.SetCommandService(_ircCommandService);

        _sessions.TryAdd(sessionId, newSession);
    }
}
