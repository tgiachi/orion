using Orion.Network.Core.Interfaces.Transports;

namespace Orion.Network.Core.Data;

public class NetworkTransportData
{
    public string Id { get; }

    public string Name { get; set; }

    public string IpAddress { get; set; }

    public int Port { get; set; }

    public INetworkTransport Transport { get; set; }

    public NetworkTransportData(INetworkTransport transport)
    {
        Id = transport.Id;
        Name = transport.Name;
        IpAddress = transport.IpAddress;
        Port = transport.Port;
        Transport = transport;
    }
}
