using System.ComponentModel.DataAnnotations;

namespace studio_api.Contracts;

public record CreateMomentRequest(
    [Required] [MaxLength(80)] string Title,
    [MaxLength(200)] string? Subtitle,
    [MaxLength(100)] string? Destination,
    [MaxLength(100)] string? Vibe
);