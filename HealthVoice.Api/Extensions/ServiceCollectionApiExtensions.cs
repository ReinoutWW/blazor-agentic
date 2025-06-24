using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using HealthVoice.Business.Extensions;
using HealthVoice.Infrastructure.Extensions;
using HealthVoice.Infrastructure.Data;
using Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace HealthVoice.Api.Extensions;

/// <summary>
/// Extension methods for configuring HealthVoice API services
/// </summary>
public static class ServiceCollectionApiExtensions
{
    /// <summary>
    /// Adds HealthVoice API services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHealthVoiceApi(this IServiceCollection services,
                                                       IConfiguration configuration)
    {
        // Core MVC/minimal API + versioning
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "HealthVoice API", Version = "v1" });
        });

        // Health checks
        services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("SqlServer") ?? "", name: "sqlserver")
                .AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is healthy"));

        // gRPC
        services.AddGrpc();

        // Business and Infrastructure layers
        services.AddHealthVoiceBusiness();

        return services;
    }

    /// <summary>
    /// Configures the HealthVoice API pipeline
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseHealthVoiceApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthVoice API V1");
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Health checks endpoints using shared extension
        app.MapHealthVoiceHealthChecks();

        // Prometheus metrics endpoint
        app.MapMetrics("/metrics");

        return app;
    }

    /// <summary>
    /// Maps health check endpoints with standardized JSON response formatting
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapHealthVoiceHealthChecks(this WebApplication app)
    {
        // Liveness check - basic availability
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false // Only basic liveness check
        });

        // Readiness check - full health with detailed JSON response
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponse
        });

        return app;
    }

    /// <summary>
    /// Writes a standardized JSON health check response
    /// </summary>
    private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            entries = report.Entries.ToDictionary(
                kvp => kvp.Key,
                kvp => new
                {
                    status = kvp.Value.Status.ToString(),
                    duration = kvp.Value.Duration.ToString(),
                    description = kvp.Value.Description ?? string.Empty,
                    data = kvp.Value.Data
                }
            )
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}