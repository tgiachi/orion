namespace Orion.Core.Server.Interfaces.Config;

public interface IOrionServerConfig
{
    Dictionary<string, IOrionSectionConfig> Sections { get; set; }
}
