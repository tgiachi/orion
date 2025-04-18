namespace Orion.Core.Server.Data.Config.Sections;

public class IrcServerConfig
{
    public OperConfig Opers { get; set; } = new();

    public PingConfig Ping { get; set; } = new();
}
