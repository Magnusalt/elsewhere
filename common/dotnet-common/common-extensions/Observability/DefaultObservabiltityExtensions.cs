using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            var settings = configuration.GetSection("DefaultWebSettings:Observability").Get<DefaultObservabilitySettings>() ??
                           throw new ArgumentNullException(nameof(configuration),
                               "Missing DefaultWebSettings:Observability");
            if (!settings.Enabled)
            {
                Console.WriteLine("Observability disabled.");
                return services;
            }

            var serviceName = settings.ServiceName ?? environment.ApplicationName;
            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(serviceName,
                    serviceVersion: typeof(DefaultObservabilityExtensions).Assembly.GetName().Version?.ToString());

                AddOpenTelemetry(services, settings, resourceBuilder);
                    

            return services;
        }

        private static void AddOpenTelemetry(IServiceCollection serviceCollection, DefaultObservabilitySettings options,
            ResourceBuilder resource)
        {
            var otlpEndpoint = options.OtlpEndpoint ?? "http://localhost:4317";

            serviceCollection.AddOpenTelemetry()
                .WithLogging(builder =>
                {
                    builder
                        .SetResourceBuilder(resource)
                        .AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri(otlpEndpoint);
                        });
                })
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(resource)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                })
                .WithMetrics(builder =>
                {
                    builder
                        .SetResourceBuilder(resource)
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                });
        }
    }
}