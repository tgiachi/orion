namespace Orion.Core.Server.Data.Config.Sections;

public class IrcServerConfig
{
    public string Host { get; set; } = "irc.orion.io";

    public AdminConfig Admin { get; set; } = new();
}
