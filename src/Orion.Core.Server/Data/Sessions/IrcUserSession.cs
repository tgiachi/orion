using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Core.Server.Data.Sessions;

public class IrcUserSession : IDisposable
{
    public string SessionId { get; set; }

    public DateTime LastActivity { get; set; }

    public DateTime LastPing { get; set; }

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
    }


    public void Dispose()
    {
    }
}
