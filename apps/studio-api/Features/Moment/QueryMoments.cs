using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using studio_api.Contracts;
using studio_api.Data;
using studio_api.Models;

namespace studio_api.Features.Moment;

public static class QueryMoments
{
    public static RouteGroupBuilder MapQueryRoute(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapGet("/{id:guid}",
            ([FromRoute] Guid id, [FromServices] StudioDbContext context) => Handle(id, context));
        routeBuilder.MapPost("/query",
            ([FromBody] QueryMomentsRequest request, [FromServices] StudioDbContext context) =>
                Handle(request, context));

        return routeBuilder;
    }

    private static async Task<IResult> Handle(Guid id, StudioDbContext context)
    {
        var moment = await context.Moments.AsNoTracking()
            .FirstOrDefaultAsync(m => m.PublishState == PublishState.Published && m.Id == id);
        return moment == null ? Results.NotFound() : Results.Ok(new QueryMomentsResponse(moment));
    }

    private static async Task<IResult> Handle(QueryMomentsRequest request, StudioDbContext context)
    {
        if (request.MomentIds.Count == 0) return Results.NoContent();

        if (request.MomentIds.Count > 20) return Results.BadRequest("Too many moments requested");

        var moments = await context.Moments
            .Where(m => m.PublishState == PublishState.Published && request.MomentIds.Contains(m.Id))
            .Include(m => m.Images)
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(moments.Select(m => new QueryMomentsResponse(m)));
    }

    public record QueryMomentsRequest(List<Guid> MomentIds);

    public record QueryMomentsResponse(
        Guid Id,
        string Title,
        string Destination,
        string Subtitle,
        string Vibe,
        List<ApiImage> Images,
        DateTime CreatedAt)
    {
        public QueryMomentsResponse(Models.Moment moment) : this(
            moment.Id,
            moment.Title,
            moment.Destination,
            moment.Subtitle,
            moment.Vibe,
            moment.Images.Select(i => new ApiImage(i.Id, i.Url, i.Order, i.Caption)).ToList(),
            moment.CreatedAt)
        {
        }
    }
}