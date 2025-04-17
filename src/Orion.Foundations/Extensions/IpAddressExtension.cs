using System.Net;

namespace Orion.Foundations.Extensions;

/// <summary>
/// Provides extension methods for IP address operations.
/// </summary>
public static class IpAddressExtension
{
    /// <summary>
    /// Converts a string representation to an IPAddress object.
    /// </summary>
    /// <param name="ip">The string representation of an IP address.</param>
    /// <returns>An IPAddress object representing the IP address.</returns>
    /// <exception cref="FormatException">Thrown when the input string is not a valid IP address format.</exception>
    /// <remarks>
    /// Handles special cases:
    /// - "*" returns IPAddress.Any (0.0.0.0)
    /// - "::" returns IPAddress.IPv6Any (::)
    /// - "*.*.*.*" returns IPAddress.Any (0.0.0.0)
    /// For all other inputs, attempts standard IP address parsing.
    /// </remarks>
    /// <example>
    /// "127.0.0.1".ToIpAddress() returns the loopback address
    /// "*".ToIpAddress() returns IPAddress.Any (0.0.0.0)
    /// "::1".ToIpAddress() returns the IPv6 loopback address
    /// </example>
    public static IPAddress ToIpAddress(this string ip)
    {
        switch (ip)
        {
            case "*":
                return IPAddress.Any;
            case "::":
                return IPAddress.IPv6Any;
        }

        if (IPAddress.TryParse(ip, out var ipAddress))
        {
            return ipAddress;
        }

        if (ip == "*.*.*.*")
        {
            return IPAddress.Any;
        }

        throw new FormatException($"Invalid IP address format: {ip}");
    }
}