using Orion.Network.Core.Utils;

namespace Orion.Network.Core.Extensions;

public static class SessionExtension
{
    public static string ToShortSessionId(this string sessionId)
    {
        return SessionUtils.GetShortSessionId(sessionId);
    }
}
