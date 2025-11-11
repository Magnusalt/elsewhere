using common_extensions.Observability;
using common_extensions.ProblemDetails;
using common_extensions.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace common_extensions;

public static class WebDefaults
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddDefaults()
        {
            builder.WebHost.UseKestrel();

            builder.Services
                .AddOptions<DefaultWebSettings>()
                .Bind(builder.Configuration.GetSection(DefaultWebSettings.DefaultSettingsRoot))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services
                .AddDefaultObservability(builder.Configuration, builder.Environment)
                .AddDefaultProblemDetails();

            return builder;
        }
    }
}