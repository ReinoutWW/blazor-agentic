using HealthVoice.Domain.Entities;
using HealthVoice.Api.Endpoints;
using Healthvoice.V1;

namespace HealthVoice.Api.Mappers;

/// <summary>
/// Extension methods for mapping Patient domain entities to API response models
/// </summary>
public static class PatientMappers
{
    /// <summary>
    /// Maps a Patient domain entity to a REST API PatientResponse
    /// </summary>
    /// <param name="patient">The patient domain entity</param>
    /// <returns>A PatientResponse for REST API</returns>
    public static HealthVoice.Api.Endpoints.PatientResponse ToRestResponse(this Patient patient)
    {
        return new HealthVoice.Api.Endpoints.PatientResponse
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            DateOfBirth = patient.DateOfBirth,
            CreatedAt = patient.CreatedAt,
            FullName = patient.FullName
        };
    }

    /// <summary>
    /// Maps a Patient domain entity to a gRPC PatientResponse
    /// </summary>
    /// <param name="patient">The patient domain entity</param>
    /// <returns>A PatientResponse for gRPC API</returns>
    public static Healthvoice.V1.PatientResponse ToGrpcResponse(this Patient patient)
    {
        return new Healthvoice.V1.PatientResponse
        {
            PatientId = patient.Id.ToString(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            DateOfBirth = patient.DateOfBirth.ToString("yyyy-MM-dd"),
            CreatedAt = patient.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            FullName = patient.FullName
        };
    }
} 