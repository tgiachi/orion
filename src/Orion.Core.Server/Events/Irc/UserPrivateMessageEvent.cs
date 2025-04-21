using Orion.Irc.Core.Data.Messages;

namespace Orion.Core.Server.Events.Irc;

/// <summary>
/// This event is triggered when a user sends a private message to another user (the aim of the message is not to be spyed, but for put antispam ).
/// </summary>
/// <param name="Source"></param>
/// <param name="Target"></param>
/// <param name="Message"></param>
/// <param name="TargetType"></param>
public record UserPrivateMessageEvent(string Source, string Target, string Message, PrivMessageTarget.TargetType TargetType);
