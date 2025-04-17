using Orion.Foundations.Utils;

namespace Orion.Foundations.Extensions;

public static class YamlMethodExtension
{
    public static string ToYaml(this object obj)
    {
        return YamlUtils.Serialize(obj);
    }

    public static T? FromYaml<T>(this string yaml)
    {
        return YamlUtils.Deserialize<T>(yaml);
    }

    public static object? FromYaml(this string yaml, Type type)
    {
        return YamlUtils.Deserialize(yaml, type);
    }
}
