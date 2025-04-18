namespace Orion.Core.Server.Data.Config.Sections;

public class PingConfig
{
    public int Interval { get; set; } = 60;

    public int Timeout { get; set; } = 30;
}
