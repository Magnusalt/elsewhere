using common_extensions;
using studio_api.Data;
using studio_api.Features;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

builder.Services.AddNpgsql<StudioDbContext>(builder.Configuration.GetConnectionString("studio"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudioDbContext>();

    try
    {
        db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to initialize database.");
    }
}

app.MapMomentRoutes();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();