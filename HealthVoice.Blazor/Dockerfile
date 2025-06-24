# Multi-stage Dockerfile for HealthVoice Blazor App
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

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

# For demo purposes, run as root to avoid SQLite permission issues
# In production, use proper volume mounts and permissions

ENTRYPOINT ["dotnet", "HealthVoice.Blazor.dll"] 