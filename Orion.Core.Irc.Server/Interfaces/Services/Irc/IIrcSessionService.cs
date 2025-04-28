using Orion.Core.Irc.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Services.Base;

namespace Orion.Core.Irc.Server.Interfaces.Services.Irc;

public interface IIrcSessionService : IOrionService
{
    List<IrcUserSession> Sessions { get; }

    IrcUserSession? GetSession(string sessionId, bool throwIfNotFound = true);

    List<IrcUserSession> QuerySessions(Func<IrcUserSession, bool> predicate);

    IrcUserSession? FindByNickName(string nickName);

    int TotalSessions { get; }
    int TotalInvisibleSessions { get; }

    int MaxSessions { get; }

    int TotalOpers { get; }


}
