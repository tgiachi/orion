using CommandLine;
using Orion.Core.Server.Interfaces.Options;

using Orion.Foundations.Types;

namespace Orion.Core.Server.Data.Options;

public class OrionServerOptions : IOrionServerCmdOptions
{
    [Option('c', "config", Required = false, HelpText = "Path to the configuration file.")]
    public string? ConfigFile { get; set; }

    [Option('r', "root-directory", Required = false, HelpText = "Root directory for the server.")]
    public string RootDirectory { get; set; } = null!;

    [Option('s', "show-header", Required = false, HelpText = "Show the header in the console.")]
    public bool ShowHeader { get; set; } = true;

    [Option('l', "log-level", Required = false, HelpText = "Log level for the server.")]
    public LogLevelType LogLevel { get; set; } = LogLevelType.Information;

    [Option('v', "verbose", Required = false, HelpText = "Verbose output.")]
    public bool IsDebug { get; set; }
}
