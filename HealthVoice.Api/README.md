# HealthVoice API

A comprehensive REST and gRPC API for the HealthVoice patient management system, built with ASP.NET Core 8 following clean architecture principles.

## 🏗️ Architecture

The API layer sits as a presentation layer above the Business logic layer, exposing the existing services via:
- **REST endpoints** for broad client compatibility
- **gRPC services** for high-performance scenarios
- **Health checks** for monitoring and observability
- **Prometheus metrics** for operational insights

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQLite (for development) or SQL Server (for production)

### Run Locally
```bash
cd HealthVoice.Api
dotnet run --urls "http://localhost:5000"
```

The API will be available at:
- **Swagger UI**: http://localhost:5000/swagger
- **Health Checks**: http://localhost:5000/health/ready
- **Metrics**: http://localhost:5000/metrics

## 📡 REST API Endpoints

### Patients

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/patients` | Get all patients |
| GET | `/api/v1/patients/{id}` | Get patient by ID |
| POST | `/api/v1/patients` | Create new patient |

### Example: Create Patient
```bash
curl -X POST "http://localhost:5000/api/v1/patients" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe", 
    "email": "john.doe@example.com",
    "dateOfBirth": "1990-01-15"
  }'
```

## 🔧 gRPC Services

### Patients Service
- **GetPatient**: Retrieve patient by ID
- **CreatePatient**: Create new patient
- **GetAllPatients**: Get all patients

### Example: gRPC Client (C#)
```csharp
var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new Patients.PatientsClient(channel);

var request = new GetPatientRequest { PatientId = "123e4567-e89b-12d3-a456-426614174000" };
var response = await client.GetPatientAsync(request);
```

## 🏥 Health Checks

- **Liveness**: `/health/live` - Basic API availability
- **Readiness**: `/health/ready` - API + dependencies status

Example response:
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "duration": "00:00:00.0009672"
    },
    {
      "name": "api", 
      "status": "Healthy",
      "duration": "00:00:00.0009739"
    }
  ]
}
```

## 📊 Observability Stack

### Prometheus Metrics
Available at `/metrics` endpoint, includes:
- HTTP request metrics
- Response time percentiles
- Error rates
- Custom business metrics

### Monitoring Setup
Run the full monitoring stack:
```bash
docker-compose -f docker-compose.monitoring.yml up -d
```

This provides:
- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000 (admin/healthvoice123)
- **Health Checks UI**: http://localhost:8080

## 🐳 Docker Deployment

### Build Image
```bash
docker build -f HealthVoice.Api/Dockerfile -t healthvoice-api:latest .
```

### Run Container
```bash
docker run -p 5000:80 healthvoice-api:latest
```

## 🔧 Configuration

### Development (appsettings.Development.json)
```json
{
  "ConnectionStrings": {
    "Sqlite": "Data Source=healthvoice.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Production Environment Variables
- `ConnectionStrings__SqlServer`: SQL Server connection string
- `ASPNETCORE_ENVIRONMENT`: Set to `Production`

## 📝 API Versioning

The API supports versioning through:
- URL segments: `/api/v1/patients`
- Header: `X-Version: 1.0`
- Accept header: `application/json;ver=1.0`

## 🔐 Security Notes

- gRPC uses TLS in production
- Health check endpoints are public
- Metrics endpoint should be secured in production
- Rate limiting configured via `AspNetCoreRateLimit`

## 🧪 Testing

### Integration Tests
```bash
dotnet test tests/HealthVoice.Api.Integration.Tests
```

### Load Testing
```bash
# Using k6 (install k6 first)
k6 run scripts/load-test.js
```

## 📋 Implementation Checklist

✅ REST API endpoints with OpenAPI/Swagger  
✅ gRPC services with protobuf contracts  
✅ Health checks (liveness + readiness)  
✅ Prometheus metrics integration  
✅ API versioning support  
✅ Docker containerization  
✅ Monitoring stack (Prometheus + Grafana)  
✅ Clean architecture compliance  
✅ Extension method pattern  

## 🔗 Related Documentation

- [API Layer Strategy Guide](../contracts/openapi/v1.yaml)
- [gRPC Contracts](../contracts/protobuf/patients.proto)
- [Business Layer Documentation](../HealthVoice.Business/README.md)
- [Infrastructure Setup](../HealthVoice.Infrastructure/README.md)

## 🐛 Troubleshooting

### Common Issues

**gRPC not working**: Ensure HTTP/2 is enabled and using HTTPS in production

**Health checks failing**: Check database connectivity and required services

**Metrics not appearing**: Verify `/metrics` endpoint is accessible and Prometheus scraping config

**Build errors**: Ensure all NuGet packages are restored (`dotnet restore`)

---

Built with ❤️ following HealthVoice architecture guidelines. 