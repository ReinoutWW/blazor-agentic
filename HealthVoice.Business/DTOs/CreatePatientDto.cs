namespace HealthVoice.Business.DTOs;

/// <summary>
/// Data transfer object for creating a new patient
/// </summary>
public record CreatePatientDto(
    string FirstName,
    string LastName,
    string Email,
    DateOnly DateOfBirth
);