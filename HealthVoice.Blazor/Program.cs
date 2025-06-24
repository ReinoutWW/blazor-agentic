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

await app.RunAsync();
