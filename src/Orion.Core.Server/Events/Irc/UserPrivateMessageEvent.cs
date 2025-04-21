using Orion.Irc.Core.Data.Messages;

namespace Orion.Core.Server.Events.Irc;

public record UserPrivateMessageEvent(string Source, string Target, string Message, PrivMessageTarget.TargetType TargetType);
