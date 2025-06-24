using FluentValidation;
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
        // Register all services ending with "Service" from the Business assembly
        var businessAssembly = typeof(ServiceCollectionBusinessExtensions).Assembly;
        
        // Register services
        services.Scan(scan => scan
            .FromAssemblies(businessAssembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service", StringComparison.Ordinal)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(businessAssembly);

        return services;
    }
}