using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Interfaces.Options;
using Serilog;

namespace Orion.Core.Server.Data.Internal;

public class AppContextData
{
    public string AppName { get; set; }

    public string Environment { get; set; }

    public DirectoriesConfig DirectoriesConfig { get; set; }

    public IOrionServerCmdOptions ServerOptions { get; set; }


    public LoggerConfiguration LoggerConfiguration { get; set; }
}
