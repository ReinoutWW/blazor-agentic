using FluentValidation;
using HealthVoice.Business.DTOs;
using HealthVoice.Domain.Contracts;
using HealthVoice.Domain.Entities;

namespace HealthVoice.Business.Services;

/// <summary>
/// Patient business service
/// </summary>
public class PatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public PatientService(IUnitOfWork unitOfWork, IClock clock)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <summary>
    /// Creates a new patient
    /// </summary>
    /// <param name="dto">Patient creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created patient's ID</returns>
    public async Task<Guid> CreatePatientAsync(CreatePatientDto dto, CancellationToken cancellationToken = default)
    {
        // Check for cancellation at the start
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(dto);

        // Validate using FluentValidation
        var validator = new Validators.CreatePatientDtoValidator();
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"Validation failed: {errors}");
        }

        var patient = new Patient
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            DateOfBirth = dto.DateOfBirth,
            CreatedAt = _clock.UtcNow
        };

        var patientRepo = _unitOfWork.Repo<Patient>();
        await patientRepo.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return patient.Id;
    }

    /// <summary>
    /// Gets all patients
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of patients</returns>
    public async Task<IEnumerable<Patient>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
    {
        var patientRepo = _unitOfWork.Repo<Patient>();
        return await patientRepo.GetAllAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a patient by ID
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Patient if found, null otherwise</returns>
    public async Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patientRepo = _unitOfWork.Repo<Patient>();
        return await patientRepo.GetAsync(id, cancellationToken);
    }
}