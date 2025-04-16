namespace Orion.Core.Server.Interfaces.Config;

public interface IOrionSectionConfig
{
    void Load();

    void BeforeSave();
}
