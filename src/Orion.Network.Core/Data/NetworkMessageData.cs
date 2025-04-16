namespace Orion.Network.Core.Data;

public struct NetworkMessageData(string sessionId, string message)
{
    public string SessionId { get; set; } = sessionId;

    public string Message { get; set; } = message;
}
