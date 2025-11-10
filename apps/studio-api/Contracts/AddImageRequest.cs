using System.ComponentModel.DataAnnotations;

namespace studio_api.Contracts;

public record AddImageRequest(
    [Required] string Url,
    [Required] string Caption,
    [Required] int Order
);