using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Interfaces.Config;

namespace Orion.Core.Server.Data.Config;

public class OrionServerConfig : IOrionServerConfig
{
    public Dictionary<string, IOrionSectionConfig> Sections { get; set; } = new();

    public OrionServerConfig()
    {
        AddSection<WebHttpConfig>();
        AddSection<ServerConfig>();
        AddSection<NetworkConfig>();
        AddSection<OperConfig>();
        AddSection<ProcessConfig>();
    }

    private void AddSection<TSection>() where TSection : IOrionSectionConfig, new()
    {
        var section = new TSection();
        var sectionName = typeof(TSection).Name.Replace("Config", string.Empty);
        if (!Sections.TryAdd(sectionName, section))
        {
            throw new Exception($"Section {sectionName} already exists.");
        }
    }
}
