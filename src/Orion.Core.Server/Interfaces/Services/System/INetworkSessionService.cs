using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Core.Server.Interfaces.Sessions;

namespace Orion.Core.Server.Interfaces.Services.System;

public interface INetworkSessionService<TSession> : IOrionService
    where TSession : INetworkSession, new()
{
    List<TSession> Sessions { get; }

    TSession? GetSession(string sessionId, bool throwIfNotFound = true);

    IEnumerable<TSession> QuerySessions(Func<TSession, bool> predicate);

    int TotalSessions { get; }

    TSession AddSession(string? sessionId = null);

    void RemoveSession(string sessionId);
}
