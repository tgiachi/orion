using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Orion.Core.Utils;

public static class YamlUtils
{
    private static readonly INamingConvention defaultNamingConvention = UnderscoredNamingConvention.Instance;

    public static T? Deserialize<T>(string yaml, StaticContext? staticContext = null)
    {
        if (staticContext != null)
        {
            var staticDeserializer = new StaticDeserializerBuilder(staticContext)
                .WithNamingConvention(defaultNamingConvention)
                .Build();

            return staticDeserializer.Deserialize<T>(yaml);
        }


        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(defaultNamingConvention)
            .Build();

        return deserializer.Deserialize<T>(yaml);
    }

    public static object? Deserialize(string yaml, Type type, StaticContext? staticContext = null)
    {
        if (staticContext != null)
        {
            var staticDeserializer = new StaticDeserializerBuilder(staticContext)
                .WithNamingConvention(defaultNamingConvention)
                .Build();

            return staticDeserializer.Deserialize(yaml, type);
        }

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(defaultNamingConvention)
            .Build();

        return deserializer.Deserialize(yaml, type);
    }

    public static string Serialize<T>(T obj, StaticContext? staticContext = null)
    {
        if (staticContext != null)
        {
            var staticSerializer = new StaticSerializerBuilder(staticContext)
                .WithNamingConvention(defaultNamingConvention)
                .Build();

            return staticSerializer.Serialize(obj);
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(defaultNamingConvention)
            .Build();

        return serializer.Serialize(obj);
    }
}
