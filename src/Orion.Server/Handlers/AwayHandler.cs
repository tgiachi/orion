using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class AwayHandler : BaseIrcCommandListener, IIrcCommandHandler<AwayCommand>
{
    public AwayHandler(ILogger<AwayHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        RegisterCommandHandler(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, AwayCommand command
    )
    {
        if (!string.IsNullOrEmpty(command.Message))
        {
            session.SetAway(command.Message);

            await session.SendCommandAsync(
                RplNowaway.Create(ServerHostName, session.NickName, command.Message)
            );
        }
        else
        {
            await session.SendCommandAsync(
                RplUnaway.Create(ServerHostName, session.NickName)
            );

            session.SetBack();
        }
    }
}
