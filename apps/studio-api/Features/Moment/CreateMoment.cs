using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using studio_api.Data;
using studio_api.Models;

namespace studio_api.Features.Moment;

public static class CreateMoment
{
    public static RouteGroupBuilder MapCreateRoute(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapPost("/", Handle);

        return routeBuilder;
    }

    private static async Task<IResult> Handle(HttpContext context, [FromBody] CreateMomentRequest request,
        [FromServices] StudioDbContext dbContext)
    {
        // TODO: create a middleware that adds this to context.User
        var userId = context.Request.Headers["X-User-Id"].ToString();
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var entity = await dbContext.Moments.AddAsync(new Models.Moment
        {
            Title = request.Title,
            Destination = request.Destination ?? string.Empty,
            Subtitle = request.Subtitle ?? string.Empty,
            Vibe = request.Vibe ?? string.Empty,
            OwnerId = userId,
            PublishState = PublishState.Draft
        });

        await dbContext.SaveChangesAsync();

        return Results.Ok(new CreateMomentResponse(entity.Entity.Id));
    }

    public record CreateMomentRequest(
        [Required] [MaxLength(80)] string Title,
        [MaxLength(200)] string? Subtitle,
        [MaxLength(100)] string? Destination,
        [MaxLength(100)] string? Vibe
    );

    public record struct CreateMomentResponse(Guid MomentId);
}