using HealthVoice.Api.Extensions;
using HealthVoice.Api.Endpoints;
using HealthVoice.Api.Services;
using HealthVoice.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure services following HealthVoice architecture
builder.Services
    .AddHealthVoiceInfrastructure(builder.Configuration, builder.Environment)
    .AddHealthVoiceApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHealthVoiceApi();

// Map REST API endpoints
app.MapPatientEndpoints();

// Map gRPC services
app.MapGrpcService<PatientsGrpcService>();

await app.RunAsync();
