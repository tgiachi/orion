namespace Orion.Core.Server.Data.Config.Sections;

public class IrcSupportConfig
{
    public int TopicLength { get; set; } = 300;
    public int NickLength { get; set; } = 30;
    public int AwayLength { get; set; } = 200;
    public int ChanLength { get; set; } = 200;
    public int UserLength { get; set; } = 30;
    public int MaxTargets { get; set; } = 10;

    public int HostLength { get; set; } = 64;
    public string ChanModes { get; set; } = "eIbq,k,flj,CFLMPQScgimnprstz";


    /// <summary>
    /// User modes supported by this server
    /// </summary>
    public string UserModes { get; set; } = "iwos";

    /// <summary>
    /// Channel modes supported by this server
    /// </summary>
    public string ChannelModes { get; set; } = "bklmntsiIpK";
}
