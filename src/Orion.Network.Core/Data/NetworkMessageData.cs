using Orion.Core.Types;

namespace Orion.Network.Core.Data;

public struct NetworkMessageData(string sessionId, string message, ServerNetworkType serverNetworkType)
{
    public string SessionId { get; set; } = sessionId;

    public string Message { get; set; } = message;
    
    public ServerNetworkType ServerNetworkType { get; set; } = serverNetworkType;
}
