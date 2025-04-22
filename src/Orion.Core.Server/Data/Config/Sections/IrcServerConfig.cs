using Orion.Foundations.Utils;

namespace Orion.Core.Server.Data.Config.Sections;

public class IrcServerConfig
{
    public OperConfig Opers { get; set; } = new();
    public PingConfig Ping { get; set; } = new();
    public string ServerPassword { get; set; }

    public IrcSupportConfig Limits { get; set; } = new();

    /// <summary>
    ///  Motd (Message of the Day) for the server
    ///  use file:// to load from a file (from orion root) and [CRLF] for line breaks
    /// </summary>
    public string Motd { get; set; } =
        "Welcome to Orion IRC Server[CRLF]This is a test server[CRLF]Please use /help for more information";



}
