using common_extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();
