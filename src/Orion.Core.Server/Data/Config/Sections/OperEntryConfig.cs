using Orion.Foundations.Utils;

namespace Orion.Core.Server.Data.Config.Sections;

public class OperEntryConfig
{
    public string Host { get; set; } = "*@*";

    public string VHost { get; set; } = "opers.orion.io";

    public string PasswordHash { get; set; } = "hash://" + HashUtils.CreatePassword("password");

    public string NickName { get; set; } = "Oper";
    

}
