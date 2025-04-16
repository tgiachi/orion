namespace Orion.Network.Core.Interfaces.Transports;

public interface INetworkTransport
{
    /// <summary>
    /// Delegate for handling client connection events
    /// </summary>
    /// <param name="transportName">The name of transport</param>
    /// <param name="sessionId">The unique identifier for the client session</param>
    /// <param name="endpoint">The network endpoint of the connected client</param>
    public delegate void ClientConnectedHandler(string transportName, string sessionId, string endpoint);

    /// <summary>
    /// Delegate for handling client disconnection events
    /// </summary>
    /// <param name="transportName">The name of transport</param>
    /// <param name="sessionId">The unique identifier for the client session</param>
    /// <param name="endpoint">The network endpoint of the disconnected lient</param>
    public delegate void ClientDisconnectedHandler(string transportName, string sessionId, string endpoint);

    /// <summary>
    /// Delegate for handling received message events
    /// </summary>
    /// <param name="transportName">The name of transport</param>
    /// <param name="sessionId">The unique identifier for the client session</param>
    /// <param name="data">The raw message data received from the client</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    public delegate void MessageReceivedHandler(
        string transportName, string sessionId, ReadOnlyMemory<byte> data
    );

    /// <summary>
    /// Event triggered when a client connects to the server
    /// </summary>
    event ClientConnectedHandler ClientConnected;

    /// <summary>
    /// Event triggered when a client disconnects from the server
    /// </summary>
    event ClientDisconnectedHandler ClientDisconnected;

    /// <summary>
    /// Event triggered when a message is received from a client
    /// </summary>
    event MessageReceivedHandler MessageReceived;

    /// <summary>
    ///   The transport name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Starts the transport.
    /// </summary>
    Task StartAsync();

    /// <summary>
    ///     Stops the transport.
    /// </summary>
    Task StopAsync();

    /// <summary>
    ///   Sends a message to the transport.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendAsync(string sessionId, byte[] message, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Check if the transport have a session
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    bool HaveSession(string sessionId);
}
