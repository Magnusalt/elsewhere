using common_extensions;
using Microsoft.AspNetCore.Mvc;
using Qdrant.Client;
using recommender_api.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults();

builder.Services.AddSingleton(new QdrantClient(builder.Configuration.GetValue<string>("Qdrant:Host")!,
    builder.Configuration.GetValue<int>("Qdrant:Port")));

builder.Services.AddHttpClient("Embeddings",
    client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Embeddings:Address")!));

var app = builder.Build();

app.MapGet("/health", (ILogger<Program> log) =>
{
    log.LogInformation("healthy");
    Results.Ok("Healthy");
});


// TODO: break this out into a feature
app.Map("/search", async ([FromQuery] string query, [FromServices] QdrantClient qdrantClient,
    [FromServices] IHttpClientFactory factory) =>
{
    // TODO: create a shared embeddings client in common lib.
    var client = factory.CreateClient("Embeddings");
    var response = await client.PostAsJsonAsync("embed-query", new { query });
    response.EnsureSuccessStatusCode();
    var embeddingsResult = await response.Content.ReadFromJsonAsync<EmbeddingsResult>();

    if (embeddingsResult is null) return Results.NotFound("No embeddings could be generated");

    var searchResult = await qdrantClient.SearchAsync("moments", embeddingsResult.Vector.ToArray());

    return Results.Ok(searchResult);
});

app.Run();