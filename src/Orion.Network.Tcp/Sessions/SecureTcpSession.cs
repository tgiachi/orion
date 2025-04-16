using NetCoreServer;
using Orion.Network.Tcp.Servers;

namespace Orion.Network.Tcp.Sessions;

public class SecureTcpSession : SslSession
{
    private readonly SecureTcpServer _server;
    public SecureTcpSession(SecureTcpServer server) : base(server)
    {
        _server = server;
    }
}
