using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Interfaces.Services.Base;

namespace Orion.Core.Server.Interfaces.Services.Irc;

public interface IIrcSessionService : IOrionService
{
    IrcUserSession? GetSession(string sessionId, bool throwIfNotFound = true);
}
