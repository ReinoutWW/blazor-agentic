# HealthVoice - Layered Architecture Blazor Application

A healthcare-focused Blazor application demonstrating clean architecture principles with Entity Framework Core and a layered design pattern.

## 🏗️ Architecture

This application follows a strict layered architecture with clear separation of concerns:

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐     ┌──────────────┐
│   Domain        │ --> │   Business       │ --> │ Infrastructure  │ --> │    Blazor    │
│   (Entities)    │     │   (Services)     │     │   (Data/EF)     │     │    (UI)      │
└─────────────────┘     └──────────────────┘     └─────────────────┘     └──────────────┘
        ▲                        ▲                        ▲                       │
        └────────────────────────┴────────────────────────┴───────────────────────┘
                              Dependency Flow (References)
```

### Layer Responsibilities

| Layer | Project | Purpose | Dependencies |
|-------|---------|---------|--------------|
| **Domain** | `HealthVoice.Domain` | Pure POCO entities, contracts (IRepository, IUnitOfWork) | None |
| **Business** | `HealthVoice.Business` | Application services, DTOs, business logic validation | Domain |
| **Infrastructure** | `HealthVoice.Infrastructure` | EF Core, concrete repositories, database configuration | Domain |
| **UI** | `HealthVoice.Blazor` | Blazor components, pages, DI bootstrap | Business, Infrastructure |

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQLite (for development database)

### Running the Application

1. **Clone and build:**
   ```bash
   git clone <repository-url>
   cd blazor-agentic
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run --project HealthVoice.Blazor
   ```

3. **Open browser:**
   Navigate to `http://localhost:5022`

The database will be automatically created on first run.

## 🧪 Testing the Architecture

### Patient Management Demo

The application includes a **Patients** page that demonstrates the full stack:

1. **Navigate to** `/patients` in the application
2. **Add a new patient** using the form on the left
3. **View patient list** on the right side
4. **Observe the data flow:**
   - UI → Business Service → Unit of Work → Repository → Entity Framework → SQLite

### Architecture Validation

✅ **Domain Layer**: Pure entities with no external dependencies  
✅ **Business Layer**: Clean service methods with validation  
✅ **Infrastructure Layer**: EF Core implementation hidden behind interfaces  
✅ **UI Layer**: Thin Blazor components that only handle presentation  

## 📁 Project Structure

```
HealthVoice.sln
├── HealthVoice.Domain/
│   ├── Contracts/          # IRepository, IUnitOfWork, IClock
│   └── Entities/           # Patient, etc.
├── HealthVoice.Business/
│   ├── Services/           # PatientService
│   ├── DTOs/              # CreatePatientDto
│   └── Extensions/        # DI registration
├── HealthVoice.Infrastructure/
│   ├── Data/              # AppDbContext
│   ├── Repositories/      # EfRepository<T>
│   ├── UnitOfWork/        # EfUnitOfWork
│   ├── Services/          # SystemClock
│   └── Extensions/        # DI registration
└── HealthVoice.Blazor/
    ├── Components/
    │   ├── Pages/         # Patients.razor
    │   └── Layout/        # Navigation
    └── Program.cs         # DI bootstrap
```

## 🔧 Key Features Demonstrated

### Extension Method Pattern
Clean DI registration using extension methods:
```csharp
builder.Services
    .AddHealthVoiceInfrastructure(builder.Configuration, builder.Environment)
    .AddHealthVoiceBusiness();
```

### Repository Pattern
Generic repository with Unit of Work:
```csharp
var patientRepo = _unitOfWork.Repo<Patient>();
await patientRepo.AddAsync(patient);
await _unitOfWork.SaveChangesAsync();
```

### Clean Business Services
Pure business logic with dependency injection:
```csharp
public class PatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    // Clean, testable methods...
}
```

### Database-First or Code-First
- Development: SQLite database
- Production: SQL Server (configurable)
- Auto-creation on startup for demo purposes

## 🎯 Next Steps

This foundation supports adding:

- ✅ **Appointments & Scheduling**
- ✅ **Medical Records**
- ✅ **User Authentication**
- ✅ **API Controllers**
- ✅ **Integration Tests**
- ✅ **FluentValidation**
- ✅ **AutoMapper**

## 📖 Architecture Guidelines

This project follows the **HealthVoice Feature Addition Playbook** ensuring:

- Consistent layered architecture
- Clean code principles
- Testable components
- SOLID principles
- Extension-based DI registration

---

**Ready to explore healthcare software architecture!** 🏥
