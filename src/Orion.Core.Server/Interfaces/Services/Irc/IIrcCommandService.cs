using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services.Base;

using Orion.Foundations.Types;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Server.Interfaces.Services.Irc;

public interface IIrcCommandService : IOrionService
{
    void AddListener<TCommand>(IIrcCommandListener command, ServerNetworkType serverNetworkType)
        where TCommand : IIrcCommand, new();

    Task SendCommandAsync<TCommand>(string sessionId, TCommand command) where TCommand : IIrcCommand;
}
