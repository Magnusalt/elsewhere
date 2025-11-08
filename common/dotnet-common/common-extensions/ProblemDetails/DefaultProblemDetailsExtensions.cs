using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace common_extensions.ProblemDetails;

public static class DefaultProblemDetailsExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDefaultProblemDetails()
        {
            services.AddExceptionHandler<DefaultProblemDetailsHandler>();
            services.AddProblemDetails(); // registers the standard helpers

            return services;
        }
    }
}

internal sealed class DefaultProblemDetailsHandler(ILogger<DefaultProblemDetailsHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://errors.elsewhere.com/internal-server-error",
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier
            }
        };

        switch (exception)
        {
            case UnauthorizedAccessException:
                problem.Status = StatusCodes.Status403Forbidden;
                problem.Title = "Access denied.";
                problem.Type = "https://errors.elsewhere.com/forbidden";
                break;

            case ValidationException ve:
                problem.Status = StatusCodes.Status400BadRequest;
                problem.Title = "Validation failed.";
                problem.Type = "https://errors.elsewhere.com/validation-error";
                problem.Detail = ve.Message;
                break;
        }

        logger.LogError(exception, "Unhandled exception for {Path}", context.Request.Path);

        context.Response.StatusCode = problem.Status ?? 500;
        context.Response.ContentType = "application/problem+json";
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = problem,
            
        });
        return true; // marks as handled
    }
}