namespace Orion.Foundations.Extensions;

public static class DateTimeMethodEx
{
    /// <summary>
    /// Get the unix timestamp of a date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long ToUnixTimestamp(this DateTime date)
    {
        var zero = new DateTime(1970, 1, 1);
        var span = date.Subtract(zero);

        return (long)span.TotalMilliseconds;
    }

    /// <summary>
    /// Get current unix timestamp
    /// </summary>
    /// <returns></returns>
    public static long ToUnixTimestamp() => DateTime.UtcNow.ToUnixTimestamp();

    /// <summary>
    ///  Get milliseconds of a date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long GetMills(this DateTime date) => date.ToUnixTimestamp() / 1000;

    /// <summary>
    ///  Convert epoch to Date time
    /// </summary>
    /// <param name="epoch"></param>
    /// <returns></returns>
    public static DateTime FromEpoch(this long epoch) => DateTimeOffset.FromUnixTimeMilliseconds(epoch).DateTime;
}

