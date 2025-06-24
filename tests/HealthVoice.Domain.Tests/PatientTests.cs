using FluentAssertions;
using HealthVoice.Domain.Entities;

namespace HealthVoice.Domain.Tests;

/// <summary>
/// Unit tests for Patient entity following HealthVoice testing strategy.
/// Tests focus on domain invariants and behavior without external dependencies.
/// </summary>
public class PatientTests
{
    [Fact]
    public void WHEN_CreatingPatient_SHOULD_SetAllPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var dateOfBirth = new DateOnly(1990, 5, 15);
        var createdAt = DateTime.UtcNow;

        // Act
        var patient = new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            CreatedAt = createdAt
        };

        // Assert
        patient.Id.Should().Be(id);
        patient.FirstName.Should().Be(firstName);
        patient.LastName.Should().Be(lastName);
        patient.Email.Should().Be(email);
        patient.DateOfBirth.Should().Be(dateOfBirth);
        patient.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData("John", "Doe", "John Doe")]
    [InlineData("Jane", "Smith", "Jane Smith")]
    [InlineData("Dr. Robert", "Johnson", "Dr. Robert Johnson")]
    public void WHEN_GettingFullName_SHOULD_CombineFirstAndLastName(string firstName, string lastName, string expectedFullName)
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var fullName = patient.FullName;

        // Assert
        fullName.Should().Be(expectedFullName);
    }

    [Theory]
    [InlineData(1990, 5, 15, 34)] // Fixed calculation for 2024-06-24
    [InlineData(2000, 1, 1, 24)]
    [InlineData(1985, 12, 31, 38)]
    public void WHEN_CalculatingAge_SHOULD_ReturnCorrectAge(int year, int month, int day, int expectedAge)
    {
        // Arrange
        var dateOfBirth = new DateOnly(year, month, day);
        var patient = new Patient { DateOfBirth = dateOfBirth };
        var currentDate = new DateOnly(2024, 6, 24); // Fixed date for testing

        // Act
        var age = patient.CalculateAge(currentDate);

        // Assert
        age.Should().Be(expectedAge);
    }

    [Fact]
    public void WHEN_PatientIsMinor_SHOULD_IdentifyCorrectly()
    {
        // Arrange
        var minorDateOfBirth = new DateOnly(2010, 1, 1); // 14 years old
        var adultDateOfBirth = new DateOnly(1990, 1, 1); // 34 years old
        var currentDate = new DateOnly(2024, 6, 24);

        var minorPatient = new Patient { DateOfBirth = minorDateOfBirth };
        var adultPatient = new Patient { DateOfBirth = adultDateOfBirth };

        // Act
        var isMinor = minorPatient.IsMinor(currentDate);
        var isAdult = !adultPatient.IsMinor(currentDate);

        // Assert
        isMinor.Should().BeTrue("Patient should be identified as minor");
        isAdult.Should().BeTrue("Patient should be identified as adult");
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("jane.smith@hospital.org")]
    [InlineData("patient123@clinic.net")]
    public void WHEN_PatientHasValidEmail_SHOULD_AcceptEmail(string email)
    {
        // Arrange & Act
        var patient = new Patient { Email = email };

        // Assert
        patient.Email.Should().Be(email);
        patient.Email.Should().Contain("@");
        patient.Email.Should().Contain(".");
    }

    [Fact]
    public void WHEN_CreatingPatientWithDefaults_SHOULD_HaveValidState()
    {
        // Arrange & Act
        var patient = new Patient();

        // Assert
        patient.Id.Should().NotBe(Guid.Empty); // Auto-generated Guid
        patient.FirstName.Should().Be(string.Empty); // Default value from entity
        patient.LastName.Should().Be(string.Empty); // Default value from entity
        patient.Email.Should().Be(string.Empty); // Default value from entity
        patient.DateOfBirth.Should().Be(default(DateOnly));
        patient.CreatedAt.Should().Be(default(DateTime));
    }

    [Fact]
    public void WHEN_TwoPatientsHaveSameData_SHOULD_BeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var dateOfBirth = new DateOnly(1990, 5, 15);
        var createdAt = DateTime.UtcNow;

        var patient1 = new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            CreatedAt = createdAt
        };

        var patient2 = new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            CreatedAt = createdAt
        };

        // Act & Assert
        patient1.Id.Should().Be(patient2.Id);
        patient1.FirstName.Should().Be(patient2.FirstName);
        patient1.LastName.Should().Be(patient2.LastName);
        patient1.Email.Should().Be(patient2.Email);
        patient1.DateOfBirth.Should().Be(patient2.DateOfBirth);
        patient1.CreatedAt.Should().Be(patient2.CreatedAt);
    }

    [Fact]
    public void WHEN_UpdatingPatientContact_SHOULD_ReturnNewInstanceWithUpdatedData()
    {
        // Arrange
        var originalPatient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateOnly(1990, 5, 15),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var updatedPatient = originalPatient.UpdateContact("Jane", "Smith", "jane.smith@example.com");

        // Assert
        updatedPatient.Should().NotBeSameAs(originalPatient, "Should return new instance");
        updatedPatient.Id.Should().Be(originalPatient.Id, "ID should remain the same");
        updatedPatient.FirstName.Should().Be("Jane");
        updatedPatient.LastName.Should().Be("Smith");
        updatedPatient.Email.Should().Be("jane.smith@example.com");
        updatedPatient.DateOfBirth.Should().Be(originalPatient.DateOfBirth, "DateOfBirth should remain unchanged");
        updatedPatient.CreatedAt.Should().Be(originalPatient.CreatedAt, "CreatedAt should remain unchanged");
    }

    [Fact]
    public void WHEN_UpdatingContactWithNullValues_SHOULD_HandleGracefully()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var updatedPatient = patient.UpdateContact(null!, null!, null!);

        // Assert
        updatedPatient.FirstName.Should().Be(string.Empty);
        updatedPatient.LastName.Should().Be(string.Empty);
        updatedPatient.Email.Should().Be(string.Empty);
    }

    [Fact]
    public void WHEN_UpdatingContactWithWhitespace_SHOULD_TrimAndNormalizeEmail()
    {
        // Arrange
        var patient = new Patient { FirstName = "John", LastName = "Doe", Email = "john@example.com" };

        // Act
        var updatedPatient = patient.UpdateContact("  Jane  ", "  Smith  ", "  JANE.SMITH@EXAMPLE.COM  ");

        // Assert
        updatedPatient.FirstName.Should().Be("Jane");
        updatedPatient.LastName.Should().Be("Smith");
        updatedPatient.Email.Should().Be("jane.smith@example.com");
    }

    [Theory]
    [InlineData(2006, 1, 1, false)]  // Exactly 18 years old on birthday = NOT a minor
    [InlineData(2006, 1, 2, true)]   // 17 years old (birthday tomorrow) = IS a minor
    [InlineData(2005, 12, 31, false)] // 18 years old = NOT a minor
    [InlineData(1990, 5, 15, false)] // Adult = NOT a minor
    [InlineData(2010, 1, 1, true)]   // 14 years old = IS a minor
    public void WHEN_CheckingMinorStatus_SHOULD_ReturnCorrectResult(int year, int month, int day, bool expectedIsMinor)
    {
        // Arrange
        var dateOfBirth = new DateOnly(year, month, day);
        var patient = new Patient { DateOfBirth = dateOfBirth };
        var currentDate = new DateOnly(2024, 1, 1); // Fixed reference date

        // Act
        var isMinor = patient.IsMinor(currentDate);

        // Assert
        isMinor.Should().Be(expectedIsMinor);
    }
}