using FluentAssertions;
using HealthVoice.Business.DTOs;
using HealthVoice.Business.Services;
using HealthVoice.Domain.Contracts;
using HealthVoice.Domain.Entities;
using Moq;

namespace HealthVoice.Business.Tests;

/// <summary>
/// Unit tests for PatientService following HealthVoice testing strategy.
/// Tests focus on business logic with mocked dependencies (no EF or external services).
/// </summary>
public class PatientServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Patient>> _mockPatientRepository;
    private readonly Mock<IClock> _mockClock;
    private readonly PatientService _patientService;
    private readonly DateTime _fixedDateTime = new(2024, 6, 24, 10, 30, 0, DateTimeKind.Utc);

    public PatientServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPatientRepository = new Mock<IRepository<Patient>>();
        _mockClock = new Mock<IClock>();

        // Setup UnitOfWork to return the mocked repository
        _mockUnitOfWork.Setup(uow => uow.Repo<Patient>())
                      .Returns(_mockPatientRepository.Object);

        // Setup clock to return fixed time for predictable tests
        _mockClock.Setup(c => c.UtcNow)
                  .Returns(_fixedDateTime);

        _patientService = new PatientService(_mockUnitOfWork.Object, _mockClock.Object);
    }

    [Fact]
    public async Task WHEN_CreatePatientAsync_SHOULD_CreatePatientWithCorrectData()
    {
        // Arrange
        var createDto = new CreatePatientDto(
            "John",
            "Doe",
            "john.doe@example.com",
            new DateOnly(1990, 5, 15)
        );

        Patient? capturedPatient = null;
        _mockPatientRepository.Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
                             .Callback<Patient, CancellationToken>((patient, _) => capturedPatient = patient)
                             .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);

        // Act
        var result = await _patientService.CreatePatientAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        capturedPatient.Should().NotBeNull();
        capturedPatient!.Id.Should().Be(result);
        capturedPatient.FirstName.Should().Be(createDto.FirstName);
        capturedPatient.LastName.Should().Be(createDto.LastName);
        capturedPatient.Email.Should().Be(createDto.Email);
        capturedPatient.DateOfBirth.Should().Be(createDto.DateOfBirth);
        capturedPatient.CreatedAt.Should().Be(_fixedDateTime);

        // Verify interactions
        _mockPatientRepository.Verify(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WHEN_GetPatientAsync_WithValidId_SHOULD_ReturnPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var expectedPatient = new Patient
        {
            Id = patientId,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            DateOfBirth = new DateOnly(1985, 10, 20),
            CreatedAt = _fixedDateTime
        };

        _mockPatientRepository.Setup(r => r.GetAsync(patientId, It.IsAny<CancellationToken>()))
                             .ReturnsAsync(expectedPatient);

        // Act
        var result = await _patientService.GetPatientByIdAsync(patientId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expectedPatient.Id);
        result.FirstName.Should().Be(expectedPatient.FirstName);
        result.LastName.Should().Be(expectedPatient.LastName);
        result.Email.Should().Be(expectedPatient.Email);
        result.DateOfBirth.Should().Be(expectedPatient.DateOfBirth);
        result.CreatedAt.Should().Be(expectedPatient.CreatedAt);

        _mockPatientRepository.Verify(r => r.GetAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WHEN_GetPatientAsync_WithInvalidId_SHOULD_ReturnNull()
    {
        // Arrange
        var invalidPatientId = Guid.NewGuid();

        _mockPatientRepository.Setup(r => r.GetAsync(invalidPatientId, It.IsAny<CancellationToken>()))
                             .ReturnsAsync((Patient?)null);

        // Act
        var result = await _patientService.GetPatientByIdAsync(invalidPatientId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockPatientRepository.Verify(r => r.GetAsync(invalidPatientId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WHEN_GetAllPatientsAsync_SHOULD_ReturnAllPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" },
            new() { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com" }
        };

        _mockPatientRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                             .Returns(Task.FromResult<IEnumerable<Patient>>(patients));

        // Act
        var result = await _patientService.GetAllPatientsAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(patients);

        _mockPatientRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WHEN_CreatePatientAsync_WithNullDto_SHOULD_ThrowArgumentNullException()
    {
        // Arrange
        CreatePatientDto? nullDto = null;

        // Act & Assert
        await _patientService.Invoking(s => s.CreatePatientAsync(nullDto!, CancellationToken.None))
                            .Should().ThrowAsync<ArgumentNullException>()
                            .WithParameterName("dto");

        // Verify no repository calls were made
        _mockPatientRepository.Verify(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("", "Doe", "john@example.com")] // Empty FirstName
    [InlineData("John", "", "john@example.com")] // Empty LastName
    [InlineData("John", "Doe", "")] // Empty Email
    [InlineData(null, "Doe", "john@example.com")] // Null FirstName
    [InlineData("John", null, "john@example.com")] // Null LastName
    [InlineData("John", "Doe", null)] // Null Email
    public async Task WHEN_CreatePatientAsync_WithInvalidData_SHOULD_ThrowValidationException(
        string? firstName, string? lastName, string? email)
    {
        // Arrange
        var invalidDto = new CreatePatientDto(
            firstName!,
            lastName!,
            email!,
            new DateOnly(1990, 1, 1)
        );

        // Act & Assert
        await _patientService.Invoking(s => s.CreatePatientAsync(invalidDto, CancellationToken.None))
                            .Should().ThrowAsync<FluentValidation.ValidationException>();

        // Verify no repository calls were made
        _mockPatientRepository.Verify(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WHEN_CreatePatientAsync_WithFutureDateOfBirth_SHOULD_ThrowValidationException()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(_fixedDateTime.AddYears(1)); // Future date
        var invalidDto = new CreatePatientDto(
            "John",
            "Doe",
            "john@example.com",
            futureDate
        );

        // Act & Assert
        await _patientService.Invoking(s => s.CreatePatientAsync(invalidDto, CancellationToken.None))
                            .Should().ThrowAsync<FluentValidation.ValidationException>()
                            .WithMessage("*past*"); // Should mention date must be in past

        // Verify no repository calls were made
        _mockPatientRepository.Verify(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WHEN_RepositoryThrowsException_SHOULD_PropagateException()
    {
        // Arrange
        var createDto = new CreatePatientDto(
            "John",
            "Doe",
            "john@example.com",
            new DateOnly(1990, 1, 1)
        );

        var expectedException = new InvalidOperationException("Database connection failed");
        _mockPatientRepository.Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
                             .ThrowsAsync(expectedException);

        // Act & Assert
        await _patientService.Invoking(s => s.CreatePatientAsync(createDto, CancellationToken.None))
                            .Should().ThrowAsync<InvalidOperationException>()
                            .WithMessage("Database connection failed");
    }

    [Fact]
    public async Task WHEN_CancellationRequested_SHOULD_RespectCancellation()
    {
        // Arrange
        var createDto = new CreatePatientDto(
            "John",
            "Doe",
            "john@example.com",
            new DateOnly(1990, 1, 1)
        );

        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await _patientService.Invoking(s => s.CreatePatientAsync(createDto, cts.Token))
                            .Should().ThrowAsync<OperationCanceledException>();
    }
}