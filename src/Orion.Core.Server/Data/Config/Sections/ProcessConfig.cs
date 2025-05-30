using Orion.Core.Server.Data.Config.Base;


namespace Orion.Core.Server.Data.Config.Sections;

public class ProcessConfig : BaseConfigSection
{
    public string PidFile { get; set; } = "server.pid";

    public ProcessQueueConfig ProcessQueue { get; set; } = new();

}
