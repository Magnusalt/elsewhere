using common_extensions.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace common_extensions.Observability;

public static class DefaultObservabilityExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDefaultObservability(IConfiguration configuration, IHostEnvironment environment)
        {
            var settings = configuration.GetSection("DefaultWebSettings:Observability")
                               .Get<DefaultObservabilitySettings>() ??
                           throw new ArgumentNullException(nameof(configuration),
                               "Missing DefaultWebSettings:Observability");
            if (!settings.Enabled)
            {
                Console.WriteLine("Observability disabled.");
                return services;
            }

            var otlpEndpoint = settings.OtlpEndpoint ?? "http://localhost:4317";

            var serviceName = settings.ServiceName ?? environment.ApplicationName;

            services.AddOpenTelemetry()
                .ConfigureResource(r => r
                    .AddService(
                        serviceName,
                        serviceVersion: typeof(DefaultObservabilityExtensions).Assembly.GetName().Version?.ToString()))
                .WithLogging(builder =>
                {
                    builder
                        .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                })
                .WithTracing(builder =>
                {
                    builder
                        .SetSampler(new AlwaysOnSampler())
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                })
                .WithMetrics(builder =>
                {
                    builder
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                });

            return services;
        }
    }
}