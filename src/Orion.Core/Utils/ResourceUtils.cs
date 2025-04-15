using System.Reflection;

namespace Orion.Core.Utils;

/// <summary>
/// Provides utilities for working with embedded resources.
/// </summary>
public static class ResourceUtils
{
    /// <summary>
    /// Reads the content of an embedded resource as a string.
    /// </summary>
    /// <param name="resourceName">The name of the resource to read.</param>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <returns>The content of the resource as a string.</returns>
    /// <exception cref="Exception">Thrown when the resource cannot be found in the specified assembly.</exception>
    /// <remarks>
    /// This method handles resource names that may contain either forward slashes (/) or
    /// backslashes (\) by converting them to dots, which is the standard separator for
    /// resource names in .NET assemblies.
    /// </remarks>
    public static string? ReadEmbeddedResource(string resourceName, Assembly assembly)
    {
        var resourcePath = resourceName.Replace('/', '.').Replace('\\', '.');

        var fullResourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourcePath));

        if (fullResourceName == null)
        {
            throw new Exception($"Resource {resourceName} not found in assembly {assembly.FullName}");
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            throw new Exception($"Resource {resourceName} not found in assembly {assembly.FullName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
