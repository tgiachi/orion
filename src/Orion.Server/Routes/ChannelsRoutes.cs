using Orion.Core.Server.Data.Rest;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Irc.Core.Data.Channels;

namespace Orion.Server.Routes;

public static class ChannelsRoutes
{
    public static IEndpointRouteBuilder MapChannels(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("channels").WithTags("Channels").WithDescription(" Channels routes");

        group.MapGet(
                "/list",
                async (IChannelManagerService channelManagerService) =>
                {
                    var channels = channelManagerService.Channels
                        .Select(s => new ChannelEntry(s.Name, s.MemberCount, s.Topic))
                        .ToList();

                    return Results.Ok(channels);
                }
            )
            .WithDescription("Get all channels")
            .Produces<List<ChannelEntry>>()
            .RequireAuthorization();


        group.MapGet(
                "/{channel:required}",
                (string channel, IChannelManagerService channelManagerService) =>
                {
                    var channelData = channelManagerService.GetChannel('#'+channel);


                    return channelData == null ? Results.NotFound() : Results.Ok(channelData);
                }
            )
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ChannelData>()
            .WithDescription("Get a channel by name")
            .RequireAuthorization();

        return endpoints;
    }
}
