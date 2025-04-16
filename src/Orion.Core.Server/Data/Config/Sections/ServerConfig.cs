using Orion.Core.Server.Data.Config.Base;

namespace Orion.Core.Server.Data.Config.Sections;

public class ServerConfig : BaseConfigSection
{
    public string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", string.Empty);

    public string Host { get; set; } = "irc.orion.io";

    public string Network { get; set; } = "OrionNet";

    public string Description { get; set; } = "Orion IRC server";

    public AdminConfig Admin { get; set; } = new();
}
