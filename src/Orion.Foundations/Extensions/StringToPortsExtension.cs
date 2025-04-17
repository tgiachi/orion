using Orion.Foundations.Utils;

namespace Orion.Foundations.Extensions;

public static class StringToPortsExtension
{

    public static IEnumerable<int> ToPorts(this string str)
    {
        return PortToListParserUtils.ParsePorts(str);
    }

}
