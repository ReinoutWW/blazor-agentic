# Multi-stage Dockerfile for HealthVoice API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["HealthVoice.Api/HealthVoice.Api.csproj", "HealthVoice.Api/"]
COPY ["HealthVoice.Business/HealthVoice.Business.csproj", "HealthVoice.Business/"]
COPY ["HealthVoice.Infrastructure/HealthVoice.Infrastructure.csproj", "HealthVoice.Infrastructure/"]
COPY ["HealthVoice.Domain/HealthVoice.Domain.csproj", "HealthVoice.Domain/"]

# Restore dependencies
RUN dotnet restore "HealthVoice.Api/HealthVoice.Api.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/HealthVoice.Api"
RUN dotnet build "HealthVoice.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HealthVoice.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN addgroup --system --gid 1001 healthvoice
RUN adduser --system --uid 1001 healthvoice
USER healthvoice

ENTRYPOINT ["dotnet", "HealthVoice.Api.dll"] 