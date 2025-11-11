namespace studio_api.Models;

public class Moment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Vibe { get; set; } = string.Empty;
    public string OwnerId { get; init; } = string.Empty;
    public List<MomentImage> Images { get; init; } = [];
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public PublishState PublishState { get; set; }
}

public enum PublishState
{
    Draft,
    Published,
    Review
}