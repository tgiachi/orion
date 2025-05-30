using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Orion.Foundations.Text;

namespace Orion.Foundations.Utils;

/// <summary>
/// Provides utility methods for JSON serialization and deserialization with AOT (Ahead-of-Time) compilation support.
/// </summary>
public static class JsonUtils
{
    public static JsonNamingPolicy DefaultNamingPolicy => JsonNamingPolicy.CamelCase;
    
    private static JsonSerializerOptions? _jsonSerializerOptions;

    public static List<JsonConverterFactory> Converters { get; } = new();

    /// <summary>
    /// Gets the default JSON serialization settings.
    /// </summary>
    /// <returns>JSON serializer options configured with snake_case naming, indentation, and other default settings.</returns>
    public static JsonSerializerOptions GetDefaultJsonSettings()
    {
        if (_jsonSerializerOptions != null)
        {
            return _jsonSerializerOptions;
        }

        _jsonSerializerOptions ??= new JsonSerializerOptions
        {
            PropertyNamingPolicy = DefaultNamingPolicy,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        Converters.ForEach(c => { _jsonSerializerOptions.Converters.Add(c); });

        return _jsonSerializerOptions;
    }

    /// <summary>
    /// Serializes an object to JSON using explicit type information for AOT compatibility.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="jsonTypeInfo">The JSON type information for AOT serialization.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string Serialize<T>(T value, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Serialize(value, jsonTypeInfo);
    }


    /// <summary>
    ///  Serializes an object to JSON and writes it to a file.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filePath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string SerializeToFile<T>(T value, string filePath, JsonSerializerOptions? options = null)
    {
        var contents = JsonSerializer.Serialize(value, options ?? GetDefaultJsonSettings());
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        File.WriteAllText(filePath, contents);

        return contents;
    }

    /// <summary>
    ///  Deserializes JSON from a file to an object of the specified type.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T DeserializeFromFile<T>(string filePath, JsonSerializerOptions? options = null)
    {
        if (!File.Exists(filePath))
        {
            return default;
        }

        var text = File.ReadAllText(filePath, TextEncoding.UTF8);
        return JsonSerializer.Deserialize<T>(text, options ?? GetDefaultJsonSettings());
    }

    /// <summary>
    /// Serializes an object to JSON using a JsonSerializerContext for AOT compatibility.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="context">The JSON serializer context containing type information.</param>
    /// <returns>A JSON string representation of the object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the type is not registered in the provided context.</exception>
    public static string Serialize<T>(T value, JsonSerializerContext context)
    {
        // Get the appropriate JsonTypeInfo from the context
        var jsonTypeInfo = context.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>;
        if (jsonTypeInfo == null)
        {
            throw new InvalidOperationException(
                $"Type {typeof(T).Name} is not registered in the provided JsonSerializerContext"
            );
        }

        return JsonSerializer.Serialize(value, jsonTypeInfo);
    }

    /// <summary>
    /// Serializes an object to JSON using default settings. Not AOT-friendly but provided for compatibility.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, GetDefaultJsonSettings());
    }

    /// <summary>
    /// Deserializes JSON to an object using explicit type information for AOT compatibility.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="jsonTypeInfo">The JSON type information for AOT deserialization.</param>
    /// <returns>The deserialized object, or default value if deserialization fails.</returns>
    public static T? Deserialize<T>(string json, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize(json, jsonTypeInfo);
    }

    /// <summary>
    /// Deserializes JSON to an object using a JsonSerializerContext for AOT compatibility.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="context">The JSON serializer context containing type information.</param>
    /// <returns>The deserialized object, or default value if deserialization fails.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the type is not registered in the provided context.</exception>
    public static T? Deserialize<T>(string json, JsonSerializerContext context)
    {
        // Get the appropriate JsonTypeInfo from the context
        var jsonTypeInfo = context.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>;
        if (jsonTypeInfo == null)
        {
            throw new InvalidOperationException(
                $"Type {typeof(T).Name} is not registered in the provided JsonSerializerContext"
            );
        }

        return JsonSerializer.Deserialize<T>(json, jsonTypeInfo);
    }

    /// <summary>
    /// Deserializes JSON to an object using default settings. Not AOT-friendly but provided for compatibility.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized object, or default value if deserialization fails.</returns>
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, GetDefaultJsonSettings());
    }

    /// <summary>
    /// Deserializes JSON to an object of a specified type using a JsonSerializerContext for AOT compatibility.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="returnType">The type to deserialize to.</param>
    /// <param name="context">The JSON serializer context containing type information.</param>
    /// <returns>The deserialized object, or null if deserialization fails.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the type is not registered in the provided context.</exception>
    public static object? Deserialize(string json, Type returnType, JsonSerializerContext context)
    {
        // Get the appropriate JsonTypeInfo from the context
        var jsonTypeInfo = context.GetTypeInfo(returnType);
        if (jsonTypeInfo == null)
        {
            throw new InvalidOperationException(
                $"Type {returnType.Name} is not registered in the provided JsonSerializerContext"
            );
        }

        return JsonSerializer.Deserialize(json, returnType, context);
    }

    /// <summary>
    /// Deserializes JSON to an object of a specified type using default settings. Not AOT-friendly but provided for compatibility.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="returnType">The type to deserialize to.</param>
    /// <returns>The deserialized object, or null if deserialization fails.</returns>
    public static object? Deserialize(string json, Type returnType)
    {
        return JsonSerializer.Deserialize(json, returnType, GetDefaultJsonSettings());
    }
}
