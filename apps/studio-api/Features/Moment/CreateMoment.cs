using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using studio_api.Contracts;
using studio_api.Data;
using studio_api.Models;

namespace studio_api.Features;

public static class CreateMoment
{
    public static RouteGroupBuilder MapCreateRoute(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapPost("/", Handle).RequireAuthorization("Travelers");

        return routeBuilder;
    }

    private static async Task<IResult> Handle(HttpContext context, [FromBody] CreateMomentRequest request,
        [FromServices] StudioDbContext dbContext)
    {
        var userId = context.User.FindFirst("sub")?.Value
                     ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var entity = await dbContext.Moments.AddAsync(new Moment
        {
            Title = request.Title,
            Destination = request.Destination ?? string.Empty,
            Subtitle = request.Subtitle ?? string.Empty,
            Vibe = request.Vibe ?? string.Empty,
            OwnerId = userId,
            PublishState = PublishState.Draft
        });

        await dbContext.SaveChangesAsync();
        
        return Results.Ok(entity.Entity);
    }
}