using Orion.Core.Utils;

namespace Orion.Core.Extensions;

public static class EncryptExtensions
{
    public static string EncryptString(this string str, string base64Key)
    {
        var result = HashUtils.Encrypt(str, base64Key.FromBase64ToByteArray());

        return result.ToBase64();
    }

    public static string DecryptString(this string str, string base64Key)
    {
        var result = HashUtils.Decrypt(str.FromBase64ToByteArray(), base64Key.FromBase64ToByteArray());

        return result;
    }
}
