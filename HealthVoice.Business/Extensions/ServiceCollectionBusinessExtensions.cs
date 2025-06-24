using Microsoft.Extensions.DependencyInjection;
using HealthVoice.Business.Services;

namespace HealthVoice.Business.Extensions;

/// <summary>
/// Business layer service collection extensions
/// </summary>
public static class ServiceCollectionBusinessExtensions
{
    /// <summary>
    /// Adds HealthVoice business services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHealthVoiceBusiness(this IServiceCollection services)
    {
        // Register business services
        services.AddScoped<PatientService>();

        // Future: Add FluentValidation if needed
        // services.AddValidatorsFromAssembly(typeof(PatientService).Assembly);

        return services;
    }
} 