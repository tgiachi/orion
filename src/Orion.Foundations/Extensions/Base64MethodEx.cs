using System.Text;
using System.Text.RegularExpressions;

namespace Orion.Foundations.Extensions;

public static class Base64MethodEx
{
    /// <summary>
    ///  Check if a string is a base64 string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsBase64String(this string s)
    {
        s = s.Trim();
        return s.Length % 4 == 0 && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
    }

    /// <summary>
    /// Convert a base64 string to a string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToBase64(this string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));


    /// <summary>
    ///  Convert a base64 string to a byte array
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToBase64(this byte[] value) => Convert.ToBase64String(value);


    /// <summary>
    ///  Convert a base64 string to a string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FromBase64(this string value) => Encoding.UTF8.GetString(Convert.FromBase64String(value));

    /// <summary>
    ///  Convert a base64 string to a byte array
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte[] FromBase64ToByteArray(this string value) => Convert.FromBase64String(value);
}
