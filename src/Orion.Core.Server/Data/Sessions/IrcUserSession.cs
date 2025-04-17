using Orion.Core.Server.Interfaces.Services.Irc;

using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Server.Data.Sessions;

public class IrcUserSession : IDisposable
{
    public string SessionId { get; set; }

    private IIrcCommandService _ircCommandService;


    public void SetCommandService(IIrcCommandService ircCommandService)
    {
        _ircCommandService = ircCommandService;
    }


    public async Task SendCommandAsync(params IIrcCommand[] commands)
    {
        foreach (var command in commands)
        {
            await _ircCommandService.SendCommandAsync(SessionId, command);
        }
    }


    public void Dispose()
    {
    }
}
