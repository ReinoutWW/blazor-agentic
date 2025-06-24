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

// Map health check endpoints using shared extension
app.MapHealthVoiceHealthChecks();

await app.RunAsync();
