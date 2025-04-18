using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Core.Server.Data.Sessions;

public class IrcUserSession : IDisposable
{
    public string SessionId { get; set; }

    public string Endpoint
    {
        get => $"{RemoteAddress}:{RemotePort}";

        set
        {
            var parts = value.Split(':');
            if (parts.Length == 2)
            {
                RemoteAddress = parts[0];
                RemotePort = int.Parse(parts[1]);
            }
            else
            {
                throw new ArgumentException("Invalid endpoint format. Expected 'address:port'.");
            }
        }
    }

    public string RemoteAddress { get; private set; }
    public int RemotePort { get; private set; }

    public DateTime LastActivity { get; set; }
    public DateTime LastPing { get; set; }
    public bool IsUserSent { get; set; }
    public bool IsNickSent { get; set; }
    public bool IsRegistered { get; set; }

    public string NickName { get; set; }
    public string UserName { get; set; }

    public string RealName { get; set; }
    public string HostName { get; set; }
    public string? VHostName { get; set; }


    public string FullName => $"{NickName}!{UserName}@{VHostName ?? HostName}";

    public bool IsAuthenticated => IsUserSent && IsNickSent && IsRegistered;

    private IIrcCommandService _ircCommandService;

    private INetworkTransportManager _networkTransportManager;

    public IrcUserSession()
    {
        Initialize();
    }

    public void SetCommandService(IIrcCommandService ircCommandService)
    {
        _ircCommandService = ircCommandService;
    }

    public void SetNetworkTransportManager(INetworkTransportManager networkTransportManager)
    {
        _networkTransportManager = networkTransportManager;
    }


    public async Task SendCommandAsync(params IIrcCommand[] commands)
    {
        foreach (var command in commands)
        {
            await _ircCommandService.SendCommandAsync(SessionId, command);
        }
    }

    public async Task DisconnectAsync()
    {
        await _networkTransportManager.DisconnectAsync(SessionId);
    }


    public void Initialize()
    {
        LastActivity = DateTime.Now;
        LastPing = DateTime.Now;
        IsUserSent = false;
        IsNickSent = false;
        IsRegistered = false;
        NickName = string.Empty;
    }


    public void Dispose()
    {
    }
}
