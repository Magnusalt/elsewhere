using studio_api.Features.Moment;

namespace studio_api.Features;

public static class MapMomentRoutesExtensions
{
    public static IEndpointRouteBuilder MapMomentRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("moments");
        group
            .MapCreateRoute()
            .MapQueryRoute();

        return group;
    }
}