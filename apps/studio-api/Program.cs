using System.Security.Claims;
using common_extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using studio_api.Features;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

var app = builder.Build();

app.MapMomentRoutes();

app.MapGet("/debug-user", (HttpContext ctx) =>
{
    var id = ctx.Request.Headers["X-User-Id"];
    var email = ctx.Request.Headers["X-User-Email"].ToString();
    var roles = ctx.Request.Headers["X-User-Roles"].ToString();

    return Results.Json(new { id, email, roles });
});

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();