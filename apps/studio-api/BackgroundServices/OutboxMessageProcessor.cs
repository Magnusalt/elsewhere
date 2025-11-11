using System.Threading.Channels;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using studio_api.Contracts;
using studio_api.Data;
using studio_api.Models;

namespace studio_api.BackgroundServices;

public record MessageSignal(Guid MessageId);

public partial class OutboxMessageProcessor(
    Channel<MessageSignal> channel,
    QdrantClient qdrantClient,
    IServiceScopeFactory serviceScopeFactory,
    IHttpClientFactory httpClientFactory,
    ILogger<OutboxMessageProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var signal in channel.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = serviceScopeFactory.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<StudioDbContext>();

            var message =
                dbContext.OutboxMessages.FirstOrDefault(x => x.Id == signal.MessageId && x.Processed == false);
            if (message == null) return;

            switch (message.Type)
            {
                case MessageType.MomentCreated:
                    await HandleMomentCreated(message.EntityId, dbContext);
                    break;
                default:
                    throw new NotSupportedException($"Message type {message.Type} is not supported");
            }

            message.Processed = true;
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }

    // Todo: turn this into a feature?
    private async Task HandleMomentCreated(Guid momentId, StudioDbContext dbContext)
    {
        var moment = dbContext.Moments.FirstOrDefault(x => x.Id == momentId);
        if (moment == null)
        {
            LogNoMomentFoundForId(logger, momentId);
            return;
        }

        var client = httpClientFactory.CreateClient("Embeddings");

        var momentToEmbedd = new EmbeddingsMoment(moment.Title, moment.Subtitle, moment.Destination, moment.Vibe);

        var responseMessage = await client.PostAsJsonAsync("embed-moment", momentToEmbedd);
        responseMessage.EnsureSuccessStatusCode();
        var embeddingsResult = await responseMessage.Content.ReadFromJsonAsync<EmbeddingsResult>();

        if (embeddingsResult == null)
        {
            logger.LogWarning("Embeddings result is null");
            return;
        }

        await qdrantClient.UpsertAsync("moments", new List<PointStruct>
        {
            new()
            {
                Id = momentId,
                Vectors = embeddingsResult.Vector.ToArray(), Payload =
                {
                    ["Title"] = momentToEmbedd.Title,
                    ["Subtitle"] = momentToEmbedd.Subtitle,
                    ["Destination"] = momentToEmbedd.Destination,
                    ["Vibe"] = momentToEmbedd.Vibe
                }
            }
        });
    }
}