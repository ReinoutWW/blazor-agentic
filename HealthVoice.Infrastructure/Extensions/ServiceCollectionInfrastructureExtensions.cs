using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HealthVoice.Domain.Contracts;
using HealthVoice.Infrastructure.Data;
using HealthVoice.Infrastructure.Repositories;
using HealthVoice.Infrastructure.Services;
using HealthVoice.Infrastructure.UnitOfWork;

namespace HealthVoice.Infrastructure.Extensions;

/// <summary>
/// Infrastructure layer service collection extensions
/// </summary>
public static class ServiceCollectionInfrastructureExtensions
{
    /// <summary>
    /// Adds HealthVoice infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The host environment</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHealthVoiceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Database context factory
        services.AddDbContextFactory<AppDbContext>(options =>
        {
            // Prioritize SQL Server connection string if available, otherwise use SQLite
            var sqlServerConnectionString = configuration.GetConnectionString("SqlServer");
            
            if (!string.IsNullOrEmpty(sqlServerConnectionString))
            {
                options.UseSqlServer(sqlServerConnectionString);
            }
            else
            {
                var sqliteConnectionString = configuration.GetConnectionString("Sqlite") ?? "Data Source=healthvoice.db";
                options.UseSqlite(sqliteConnectionString);
            }
            
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Register repositories and unit of work
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Infrastructure utilities
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }

    /// <summary>
    /// Ensures the database is created and initialized
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static async Task<IHost> EnsureDatabaseCreatedAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        
        return app;
    }
} 