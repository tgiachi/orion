using Orion.Foundations.Utils;

namespace Orion.Core.Server.Data.Config.Sections;

public class OperEntryConfig
{
    public string Host { get; set; } = "*@*";

    public string VHost { get; set; } = "opers.orion.io";

    public string PasswordHash { get; set; } = "hash://" + HashUtils.CreatePassword("password");

    public string NickName { get; set; } = "Oper";

    public void SetPassword(string clearPassword)
    {
        PasswordHash = "hash://" + HashUtils.CreatePassword(clearPassword);
    }

    public bool IsPasswordValid(string clearPassword)
    {
        return HashUtils.VerifyPassword(clearPassword, PasswordHash);
    }

    public OperEntryConfig()
    {
        if (!PasswordHash.StartsWith("hash://"))
        {
            PasswordHash = "hash://" + HashUtils.CreatePassword(PasswordHash);
        }
    }
}
