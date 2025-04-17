using Orion.Foundations.Types;

namespace Orion.Core.Server.Interfaces.Options;

public interface IOrionServerCmdOptions
{
    string ConfigFile { get; set; }

    string RootDirectory { get; set; }

    bool ShowHeader { get; set; }

    LogLevelType LogLevel { get; set; }

    bool IsDebug { get; set; }
}
