using System.Text;

namespace Orion.Foundations.Extensions;

public static class ByteUtilsMethodEx
{
    /// <summary>
    ///  Perform Md5 checksum on a byte array
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetMd5Checksum(this byte[] value)
    {
        var stream = new MemoryStream();
        stream.Write(value, 0, value.Length);

        //important: get back to start of stream
        stream.Seek(0, SeekOrigin.Begin);

        //get a string value for the MD5 hash.
        using var md5Instance = System.Security.Cryptography.MD5.Create();
        var hashResult = md5Instance.ComputeHash(stream);

        //***I did some formatting here, you may not want to remove the dashes, or use lower case depending on your application
        return BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    ///  Perform Md5 checksum on a string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetMd5Checksum(this string value) => System.Text.Encoding.UTF8.GetBytes(value).GetMd5Checksum();


    /// <summary>
    /// Convert a byte array to a human readable string with specified byte limit.
    /// </summary>
    /// <param name="value">The byte array to convert.</param>
    /// <param name="maxBytes">Maximum number of bytes to display. Default is 10.</param>
    /// <returns>A formatted string representation of the byte array.</returns>
    public static string HumanizedContent(this byte[] value, int maxBytes = 10, bool hex = true)
    {
        if (value == null || value.Length == 0)
        {
            return "[]";
        }

        var sb = new StringBuilder();
        sb.Append('[');

        int bytesToShow = Math.Min(value.Length, maxBytes);

        for (int i = 0; i < bytesToShow; i++)
        {
            var byteValue = value[i];

            if (hex)
            {
                sb.Append($"0x{byteValue:X2}");
            }
            else
            {
                sb.Append(byteValue);
            }


            sb.Append($"0x{value[i]:X2}");

            if (i < bytesToShow - 1)
            {
                sb.Append(", ");
            }
        }

        if (value.Length > maxBytes)
        {
            sb.Append(", ...");
        }

        sb.Append(']');

        return sb.ToString();
    }
}
