namespace Orion.Network.Core.Interfaces;

public interface INetworkTransport
{
    /// <summary>
    ///     Starts the transport.
    /// </summary>
    Task StartAsync();

    /// <summary>
    ///     Stops the transport.
    /// </summary>
    Task StopAsync();
}
