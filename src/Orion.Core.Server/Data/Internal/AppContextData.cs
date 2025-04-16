using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Interfaces.Config;
using Orion.Core.Server.Interfaces.Options;
using Serilog;

namespace Orion.Core.Server.Data.Internal;

public class AppContextData<TOptions, TConfig>
    where TOptions : IOrionServerCmdOptions
    where TConfig : IOrionServerConfig
{
    public string AppName { get; set; }

    public string Environment { get; set; }

    public DirectoriesConfig DirectoriesConfig { get; set; }

    public TOptions ServerOptions { get; set; }

    public TConfig ServerConfig { get; set; }

    public LoggerConfiguration LoggerConfiguration { get; set; }
}
