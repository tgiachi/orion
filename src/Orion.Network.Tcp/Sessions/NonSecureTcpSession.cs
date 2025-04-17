using System.Buffers;
using NetCoreServer;
using Orion.Network.Tcp.Servers;
using Buffer = System.Buffer;

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
        var messageBuffer = new byte[size];

        try
        {
            Buffer.BlockCopy(buffer, (int)offset, messageBuffer, 0, (int)size);
            _server.OnMessageReceived(this, messageBuffer);
        }
        catch (Exception ex)
        {
            throw;
        }

        base.OnReceived(buffer, offset, size);
    }
}
