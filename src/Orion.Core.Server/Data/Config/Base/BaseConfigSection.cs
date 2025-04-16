using Orion.Core.Server.Interfaces.Config;

namespace Orion.Core.Server.Data.Config.Base;

public class BaseConfigSection : IOrionSectionConfig
{
    public virtual void Load()
    {
    }

    public virtual void BeforeSave()
    {


    }
}
