using System.Threading.Channels;
using Orion.Network.Core.Data;
using Orion.Network.Core.Interfaces.Transports;

namespace Orion.Network.Core.Interfaces.Services;

/// <summary>
/// Manages the network transport layer for IRC communication
/// </summary>
public interface INetworkTransportManager : IDisposable
{
    /// <summary>
    /// Gets the collection of registered network transports
    /// </summary>
    List<INetworkTransport> Transports { get; }

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
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task EnqueueMessageAsync(NetworkMessageData messageData, CancellationToken cancellationToken = default);


    /// <summary>
    /// Adds a new network transport to the manager
    /// </summary>
    /// <param name="transport">The transport implementation to add</param>
    void AddTransport(INetworkTransport transport);
}
