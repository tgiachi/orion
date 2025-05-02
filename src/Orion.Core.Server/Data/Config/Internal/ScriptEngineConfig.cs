namespace Orion.Core.Server.Data.Config.Internal;

public class ScriptEngineConfig
{
    public List<string> InitScriptsFileNames { get; set; } = new List<string>() { "bootstrap.js", "index.js" };
}
