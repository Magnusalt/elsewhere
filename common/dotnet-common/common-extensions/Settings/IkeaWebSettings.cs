using System.ComponentModel.DataAnnotations;

namespace Ikea.WebDefault.Settings;

public sealed class DefaultWebSettings
{
    public static string DefaultSettingsRoot = nameof(DefaultWebSettings);

    [Required] public IkeaObservabilitySettings Observability { get; set; }
}
public sealed class DefaultObservabilitySettings
{
    public bool Enabled { get; set; } = true;
    public string? ServiceName { get; set; }
    public string? OtlpEndpoint { get; set; }
}
