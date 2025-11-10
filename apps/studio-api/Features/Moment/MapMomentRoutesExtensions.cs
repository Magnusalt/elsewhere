namespace studio_api.Features;

public static class MapMomentRoutesExtensions
{
    public static IEndpointRouteBuilder MapMomentRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("moments");
        group.MapCreateRoute();

        return app;
    }
}