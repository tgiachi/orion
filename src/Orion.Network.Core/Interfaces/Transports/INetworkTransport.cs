using Orion.Core.Types;
using Orion.Network.Core.Types;

namespace Orion.Network.Core.Interfaces.Transports;

public interface INetworkTransport
{
    /// <summary>
    /// Delegate for handling client connection events
    /// </summary>
    /// <param name="transportId">The Id of transport</param>
    /// <param name="sessionId">The unique identifier for the client session</param>
    /// <param name="endpoint">The network endpoint of the connected client</param>
    public delegate void ClientConnectedHandler(string transportId, string sessionId, string endpoint);

    /// <summary>
    /// Delegate for handling client disconnection events
    /// </summary>
    /// <param name="transportId">The Id of transport</param>
    /// <param name="sessionId">The unique identifier for the client session</param>
    /// <param name="endpoint">The network endpoint of the disconnected lient</param>
    public delegate void ClientDisconnectedHandler(string transportId, string sessionId, string endpoint);

    /// <summary>
    /// Delegate for handling received message events
    /// </summary>
    /// <param name="transportId">The id of transport</param>
    /// <param name="sessionId">The unique identifier for the client session</param>
    /// <param name="data">The raw message data received from the client</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    public delegate void MessageReceivedHandler(
        string transportId, string sessionId, ReadOnlyMemory<byte> data
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
    ///  The transport unique identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///   The transport name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///  The transport port
    /// </summary>
    int Port { get; }

    /// <summary>
    ///  The transport protocol type
    /// </summary>
    NetworkProtocolType Protocol { get; }

    /// <summary>
    ///  The transport security type
    /// </summary>
    NetworkSecurityType Security { get; }

    /// <summary>
    ///  The transport type
    /// </summary>
    ServerNetworkType ServerNetworkType { get; }

    /// <summary>
    ///  The transport IP address
    /// </summary>
    string IpAddress { get; }

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
