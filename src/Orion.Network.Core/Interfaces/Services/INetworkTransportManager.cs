using System.Threading.Channels;
using Orion.Network.Core.Data;
using Orion.Network.Core.Interfaces.Transports;

namespace Orion.Network.Core.Interfaces.Services;

/// <summary>
/// Manages the network transport layer for IRC communication
/// </summary>
public interface INetworkTransportManager : IDisposable
{
    delegate void RawPacketOutHandler(string sessionId, string transportId, byte[] data);

    delegate void RawPacketInHandler(string sessionId, string transportId, byte[] data);

    event RawPacketOutHandler RawPacketOut;
    event RawPacketInHandler RawPacketIn;

    /// <summary>
    ///  Notifies when a client connects to the network
    /// </summary>
    event INetworkTransport.ClientConnectedHandler ClientConnected;

    /// <summary>
    ///    Notifies when a client disconnects from the network
    /// </summary>
    event INetworkTransport.ClientDisconnectedHandler ClientDisconnected;

    /// <summary>
    /// Gets the collection of registered network transports
    /// </summary>
    List<NetworkTransportData> Transports { get; }

    /// <summary>
    ///  Gets the observable for network metrics
    /// </summary>
    IObservable<NetworkMetricData> NetworkMetrics { get; }

    /// <summary>
    /// Gets the channel for incoming network messages
    /// </summary>
    Channel<NetworkMessageData> IncomingMessages { get; }

    /// <summary>
    /// Gets the channel for outgoing network messages
    /// </summary>
    Channel<NetworkMessageData> OutgoingMessages { get; }

    /// <summary>
    /// Starts the network transport manager
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the network transport manager
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueues a message to be sent over the network
    /// </summary>
    /// <param name="messageData">The message data to enqueue</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task EnqueueMessageAsync(NetworkMessageData messageData);


    /// <summary>
    /// Adds a new network transport to the manager
    /// </summary>
    /// <param name="transport">The transport implementation to add</param>
    void AddTransport(INetworkTransport transport);


    /// <summary>
    ///  Gets a network transport by its ID
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    NetworkTransportData GetTransport(string Id);

    /// <summary>
    ///  Disconnects a client from the network
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    Task DisconnectAsync(string sessionId);
}
