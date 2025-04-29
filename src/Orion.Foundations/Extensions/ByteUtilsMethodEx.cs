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
    ///  Convert a byte array to a human readable string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string HumanizedContent(this byte[] value)
    {
        if (value == null || value.Length == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var b in value)
        {
            sb.Append(b.ToString("[0x{b:X2}],"));
        }

        // Remove the last comma
        if (sb.Length > 0)
        {
            sb.Remove(sb.Length - 1, 1);
        }

        return sb.ToString();
    }
}

