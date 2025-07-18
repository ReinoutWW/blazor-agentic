# Multi-stage Dockerfile for HealthVoice Blazor App
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy centralized package management file first (required for restore)
COPY ["Directory.Packages.props", "."]

# Copy project files
COPY ["HealthVoice.Blazor/HealthVoice.Blazor.csproj", "HealthVoice.Blazor/"]
COPY ["HealthVoice.Business/HealthVoice.Business.csproj", "HealthVoice.Business/"]
COPY ["HealthVoice.Infrastructure/HealthVoice.Infrastructure.csproj", "HealthVoice.Infrastructure/"]
COPY ["HealthVoice.Domain/HealthVoice.Domain.csproj", "HealthVoice.Domain/"]

# Restore dependencies
RUN dotnet restore "HealthVoice.Blazor/HealthVoice.Blazor.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/HealthVoice.Blazor"
RUN dotnet build "HealthVoice.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HealthVoice.Blazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user for security and give proper permissions
RUN addgroup --system --gid 1001 healthvoice && \
    adduser --system --uid 1001 --gid 1001 healthvoice && \
    chown -R healthvoice:healthvoice /app
USER healthvoice

# Add health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "HealthVoice.Blazor.dll"] 