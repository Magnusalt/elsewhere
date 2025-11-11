namespace studio_api.BackgroundServices;

public partial class OutboxMessageProcessor
{
    [LoggerMessage(LogLevel.Warning, "No moment found for {id}")]
    static partial void LogNoMomentFoundForId(ILogger<OutboxMessageProcessor> logger, Guid id);
}