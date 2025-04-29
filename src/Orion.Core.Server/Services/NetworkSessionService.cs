using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Interfaces.Sessions;
using Orion.Foundations.Pool;

namespace Orion.Core.Server.Services;

public class NetworkSessionService<TSession> : INetworkSessionService<TSession> where TSession : INetworkSession, new()
{
    public List<TSession> Sessions => _sessions.Values.ToList();

    public int TotalSessions => _sessions.Count;

    private readonly ILogger _logger;

    private readonly ObjectPool<TSession> _sessionsObjectPool = new();

    private readonly ConcurrentDictionary<string, TSession> _sessions = new();

    public NetworkSessionService(ILogger<NetworkSessionService<TSession>> logger)
    {
        _logger = logger;
    }


    public TSession? GetSession(string sessionId, bool throwIfNotFound = true)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            return session;
        }

        if (throwIfNotFound)
        {
            throw new KeyNotFoundException($"Session with ID {sessionId} not found.");
        }

        return default;
    }

    public IEnumerable<TSession> QuerySessions(Func<TSession, bool> predicate)
    {
        return _sessions.Values.Where(predicate);
    }


    public TSession AddSession(string? sessionId = null)
    {
        var session = _sessionsObjectPool.Get();
        session.Id = sessionId ?? Guid.NewGuid().ToString();

        session.Initialize();

        if (_sessions.TryAdd(session.Id, session))
        {
            _logger.LogDebug("Session {SessionId} added.", session.Id);
            return session;
        }


        throw new InvalidOperationException($"Session with ID {session.Id} already exists.");
    }

    public void RemoveSession(string sessionId)
    {
        if (_sessions.TryRemove(sessionId, out var session))
        {
            _logger.LogDebug("Session {SessionId} removed.", session.Id);
            session.Dispose();
            _sessionsObjectPool.Return(session);
        }
        else
        {
            _logger.LogWarning("Session {SessionId} not found for removal.", sessionId);
        }
    }
}
