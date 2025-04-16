using Microsoft.AspNetCore.Http.Json;
using Orion.Core.Utils;

namespace Orion.Core.Server.Web.Extensions;

public static class WebJsonExtension
{
    public static Action<JsonOptions> ConfigureWebJson()
    {
        return options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonUtils.GetDefaultJsonSettings().PropertyNamingPolicy;
            options.SerializerOptions.WriteIndented = JsonUtils.GetDefaultJsonSettings().WriteIndented;
            options.SerializerOptions.DefaultIgnoreCondition = JsonUtils.GetDefaultJsonSettings().DefaultIgnoreCondition;
            options.SerializerOptions.PropertyNameCaseInsensitive =
                JsonUtils.GetDefaultJsonSettings().PropertyNameCaseInsensitive;

            options.SerializerOptions.Converters.Clear();
            foreach (var converter in JsonUtils.GetDefaultJsonSettings().Converters)
            {
                options.SerializerOptions.Converters.Add(converter);
            }
        };
    }
}
