namespace Orion.Core.Server.Data.Internal;

public class IrcServerContextData
{
    /// <summary>
    ///  The name of the server.
    ///  Example: irc.example.com
    /// </summary>
    public string ServerName { get; set; }


    /// <summary>
    ///   Network name of the server.
    /// </summary>
    public string NetworkName { get; set; }


    /// <summary>
    ///  The start time of the server.
    /// </summary>
    public DateTime ServerStartTime { get; set; } = DateTime.UtcNow;
}


