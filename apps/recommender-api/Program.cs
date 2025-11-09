using common_extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

var app = builder.Build();

app.MapGet("/health", (ILogger<Program> log) =>
{
    log.LogInformation("healthy");
    Results.Ok("Healthy");
});

app.Run();