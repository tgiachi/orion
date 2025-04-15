namespace Orion.Network.Core.Interfaces;

public interface INetworkTransport
{
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
