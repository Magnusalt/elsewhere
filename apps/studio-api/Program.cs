using System.Threading.Channels;
using common_extensions;
using Qdrant.Client;
using studio_api.BackgroundServices;
using studio_api.Data;
using studio_api.Features;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

var outboxMessageChannel = Channel.CreateUnbounded<MessageSignal>();
builder.Services.AddSingleton(outboxMessageChannel);
builder.Services.AddHostedService<OutboxMessageProcessor>();

builder.Services.AddSingleton(new QdrantClient(builder.Configuration.GetValue<string>("Qdrant:Host")!,
    builder.Configuration.GetValue<int>("Qdrant:Port")));

builder.Services.AddHttpClient("Embeddings",
    client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Embeddings:Address")!));

builder.Services.AddNpgsql<StudioDbContext>(builder.Configuration.GetConnectionString("studio"));

var app = builder.Build();

// TODO: break out from Program.cs
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

    var qdrant = scope.ServiceProvider.GetRequiredService<QdrantClient>();
    await QdrantSetup.EnsureMomentsCollectionExists(qdrant);
}

app.MapMomentRoutes();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();