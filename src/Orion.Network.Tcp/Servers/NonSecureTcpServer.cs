using System.Net;
using NetCoreServer;
using Orion.Network.Core.Interfaces.Transports;
using Orion.Network.Tcp.Sessions;

namespace Orion.Network.Tcp.Servers;

public class NonSecureTcpServer : TcpServer, INetworkTransport
{
    public event INetworkTransport.ClientConnectedHandler? ClientConnected;
    public event INetworkTransport.ClientDisconnectedHandler? ClientDisconnected;
    public event INetworkTransport.MessageReceivedHandler? MessageReceived;

    public string Id { get; }
    public string Name { get; }
    public string IpAddress { get; }

    public NonSecureTcpServer(IPAddress address, int port) : base(address, port)
    {
        Id = Guid.NewGuid().ToString();
        Name = "NonSecureTcpServer";
        IpAddress = address.ToString();
    }


    protected override TcpSession CreateSession()
    {
        return new NonSecureTcpSession(this);
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


    protected override void OnConnecting(TcpSession session)
    {
        ClientConnected?.Invoke(Id, session.Id.ToString(), session.Socket.RemoteEndPoint.ToString());
        base.OnConnecting(session);
    }

    protected override void OnDisconnected(TcpSession session)
    {
        ClientDisconnected?.Invoke(Id, session.Id.ToString(), session.Socket.RemoteEndPoint.ToString());
        base.OnDisconnected(session);
    }

    public void OnMessageReceived(NonSecureTcpSession session, byte[] message)
    {
        MessageReceived?.Invoke(Id, session.Id.ToString(), message);
    }
}
