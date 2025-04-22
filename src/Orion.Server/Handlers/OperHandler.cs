using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc.Opers;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Foundations.Utils;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class OperHandler : BaseIrcCommandListener, IIrcCommandHandler<OperCommand>, IIrcCommandHandler<KillCommand>
{
    public OperHandler(ILogger<OperHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        RegisterCommandHandler<OperCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<KillCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, OperCommand command
    )
    {
        var exist = Config.Irc.Opers.Entries.FirstOrDefault(s => s.NickName == command.Username);


        if (exist == null || !exist.IsPasswordValid(command.Password) || !HostMaskUtils.IsHostMaskMatch(exist.Host, session.FullAddress))
        {
            await session.SendCommandAsync(
                ErrNoOperHost.Create(
                    ServerHostName,
                    session.NickName
                )
            );

            await PublishEventAsync(new OperWrongPasswordEvent(session.SessionId));

            return;
        }

        session.SetOperator(true);

        if (!string.IsNullOrEmpty(exist.VHost))
        {
            session.VHostName = exist.VHost;
        }

        await session.SendCommandAsync(
            RplYoureOper.Create(
                ServerHostName,
                session.NickName
            )
        );

        await PublishEventAsync(new OperLoggedInEvent(session.NickName));
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, KillCommand command
    )
    {
    }
}
