namespace Orion.Core.Server.Data.Config.Internal;

public class DiagnosticServiceConfig
{
    public int MetricsIntervalInSeconds { get; set; } = 60;

    public string PidFileName { get; set; } = "server.pid";
}
