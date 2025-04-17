using Orion.Core.Server.Data.Config.Base;


namespace Orion.Core.Server.Data.Config.Sections;

public class WebHttpConfig : BaseConfigSection
{

    public bool IsEnabled { get; set; } = true;

    public string ListenAddress { get; set; } = "0.0.0.0";

    public int ListenPort { get; set; } = 23021;

    public JwtAuthConfig JwtAuth { get; set; } = new();
}
