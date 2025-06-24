using HealthVoice.Business.Extensions;
using HealthVoice.Infrastructure.Extensions;
using HealthVoice.Blazor.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services using extension methods
builder.Services
    .AddHealthVoiceInfrastructure(builder.Configuration, builder.Environment)
    .AddHealthVoiceBusiness();

// Add Blazor components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add health checks for monitoring
builder.Services.AddHealthChecks()
    .AddCheck("blazor-app", () => 
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Blazor app is healthy"));

var app = builder.Build();

// Initialize database
await app.EnsureDatabaseCreatedAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map health check endpoints
app.MapHealthChecks("/health/ready", new()
{
    ResponseWriter = async (context, report) =>
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
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapHealthChecks("/health/live", new()
{
    Predicate = _ => false // Only basic liveness check
});

await app.RunAsync();
