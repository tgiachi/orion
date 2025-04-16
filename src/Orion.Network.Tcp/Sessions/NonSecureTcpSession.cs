using NetCoreServer;
using Orion.Network.Tcp.Servers;

namespace Orion.Network.Tcp.Sessions;

public class NonSecureTcpSession : TcpSession
{
    private readonly NonSecureTcpServer _server;

    public NonSecureTcpSession(NonSecureTcpServer server) : base(server)
    {
        _server = server;
    }


    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        _server.OnMessageReceived(this, buffer);

        base.OnReceived(buffer, offset, size);
    }
}
