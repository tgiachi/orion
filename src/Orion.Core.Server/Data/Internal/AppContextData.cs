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

    public DirectoriesConfig Directories { get; set; }

    public TOptions Options { get; set; }

    public TConfig Config { get; set; }

    public LoggerConfiguration LoggerConfiguration { get; set; }
}
