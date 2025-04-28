using Orion.Foundations.Types;

namespace Orion.Network.Core.Data;

public struct NetworkMessageData(string sessionId, byte[] message, ServerNetworkType serverNetworkType)
{
    public string SessionId { get; set; } = sessionId;

    public byte[] Message { get; set; } = message;

    public ServerNetworkType ServerNetworkType { get; set; } = serverNetworkType;
}
