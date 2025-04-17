using System.Text.Json.Serialization;
using Orion.Core.Utils;

namespace Orion.Core.Extensions;

/// <summary>
/// Provides extension methods for JSON serialization and deserialization operations.
/// </summary>
public static class JsonMethodExtension
{
    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="context">Optional JSON serializer context for AOT support. If null, uses the default serialization method.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string ToJson<T>(this T obj, JsonSerializerContext? context = null)
    {
        return context is null ? JsonUtils.Serialize(obj) : JsonUtils.Serialize(obj, context);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized object, or default value if deserialization fails.</returns>
    public static T? FromJson<T>(this string json) => JsonUtils.Deserialize<T>(json);

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type to deserialize to.</param>
    /// <param name="context">Optional JSON serializer context for AOT support. If null, uses the default deserialization method.</param>
    /// <returns>The deserialized object, or null if deserialization fails.</returns>
    public static object? FromJson(this string json, Type type, JsonSerializerContext? context = null)
    {
        return context == null ? JsonUtils.Deserialize(json, type) : JsonUtils.Deserialize(json, type, context);
    }


}
