namespace studio_api.Models;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public MessageType Type { get; set; }
    public Guid EntityId { get; set; }
    public bool Processed { get; set; }
}

public enum MessageType
{
    MomentCreated
}