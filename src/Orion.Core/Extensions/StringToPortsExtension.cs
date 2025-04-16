using Orion.Core.Utils;

namespace Orion.Core.Extensions;

public static class StringToPortsExtension
{

    public static IEnumerable<int> ToPorts(this string str)
    {
        return PortToListParserUtils.ParsePorts(str);
    }

}
