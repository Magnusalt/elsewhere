namespace studio_api.Models;

public class MomentImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid MomentId { get; set; }
    public Moment Moment { get; set; }
}