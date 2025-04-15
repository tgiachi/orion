namespace Orion.Core.Server.Data.Config.Sections;

public class WebHttpConfig
{
    public string ListenAddress { get; set; } = "0.0.0.0";

    public int ListenPort { get; set; } = 23021;
}
