namespace Orion.Network.Core.Utils;

public static class SessionUtils
{
    public static string GetShortSessionId(string sessionId)
    {
        return sessionId.Length > 8 ? sessionId[..8] : sessionId;
    }
}
