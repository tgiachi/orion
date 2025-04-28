using System.Net;
using System.Net.Sockets;
using NetCoreServer;
using Orion.Foundations.Types;
using Orion.Network.Core.Interfaces.Transports;
using Orion.Network.Core.Types;
using Orion.Network.Tcp.Sessions;

namespace Orion.Network.Tcp.Servers;

public class SecureTcpServer : SslServer, INetworkTransport
{
    public event INetworkTransport.ClientConnectedHandler? ClientConnected;
    public event INetworkTransport.ClientDisconnectedHandler? ClientDisconnected;
    public event INetworkTransport.MessageReceivedHandler? MessageReceived;

    public NetworkProtocolType Protocol => NetworkProtocolType.Tcp;
    public NetworkSecurityType Security => NetworkSecurityType.Ssl;

    public string Id { get; }
    public string Name { get; }

    public ServerNetworkType ServerNetworkType { get; }
    public string IpAddress { get; }

    public SecureTcpServer(string id, ServerNetworkType serverNetworkType, SslContext context, IPAddress address, int port) : base(
        context,
        address,
        port
    )
    {
        Id = Guid.NewGuid().ToString();
        Name = "SecureTcpServer";
        IpAddress = address.ToString();

        ServerNetworkType = serverNetworkType;
        OptionNoDelay = true;
        OptionReceiveBufferSize = 8192;
        OptionSendBufferSize = 8192;
    }

    protected override SslSession CreateSession()
    {
        return new SecureTcpSession(this);
    }


    public Task StartAsync()
    {
        Start();

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        Stop();

        return Task.CompletedTask;
    }

    public Task SendAsync(string sessionId, byte[] message, CancellationToken cancellationToken = default)
    {
        if (!HaveSession(sessionId))
        {
            throw new InvalidOperationException($"Session {sessionId} not found.");
        }

        var session = Sessions.FirstOrDefault(s => s.Key == Guid.Parse(sessionId));

        if (session.Value == null)
        {
            throw new InvalidOperationException($"Session {sessionId} not found.");
        }

        session.Value.Send(message);

        return Task.CompletedTask;
    }

    public bool HaveSession(string sessionId)
    {
        var session = Sessions.FirstOrDefault(s => s.Key == Guid.Parse(sessionId));

        return session.Value != null;
    }

    public Task DisconnectAsync(string sessionId)
    {
        if (!HaveSession(sessionId))
        {
            throw new InvalidOperationException($"Session {sessionId} not found.");
        }

        var session = Sessions.FirstOrDefault(s => s.Key == Guid.Parse(sessionId));

        session.Value?.Disconnect();

        return Task.CompletedTask;
    }


    protected override void OnConnecting(SslSession session)
    {
        ClientConnected?.Invoke(Id, session.Id.ToString(), session.Socket.RemoteEndPoint.ToString());
        base.OnConnecting(session);
    }

    protected override void OnDisconnected(SslSession session)
    {
        ClientDisconnected?.Invoke(Id, session.Id.ToString(), session.Socket.RemoteEndPoint.ToString());
        base.OnDisconnected(session);
    }

    public void OnMessageReceived(SecureTcpSession session, byte[] message)
    {
        MessageReceived?.Invoke(Id, session.Id.ToString(), message);
    }
}
