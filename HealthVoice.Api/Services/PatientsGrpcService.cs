using Grpc.Core;
using HealthVoice.Business.Services;
using HealthVoice.Business.DTOs;
using Healthvoice.V1;

namespace HealthVoice.Api.Services;

/// <summary>
/// gRPC service implementation for Patients
/// </summary>
public class PatientsGrpcService : Patients.PatientsBase
{
    private readonly PatientService _patientService;
    private readonly ILogger<PatientsGrpcService> _logger;

    /// <summary>
    /// Initializes a new instance of the PatientsGrpcService class
    /// </summary>
    /// <param name="patientService">The patient business service</param>
    /// <param name="logger">The logger</param>
    public PatientsGrpcService(PatientService patientService, ILogger<PatientsGrpcService> logger)
    {
        _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a patient by ID
    /// </summary>
    public override async Task<PatientResponse> GetPatient(GetPatientRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Getting patient with ID: {PatientId}", request.PatientId);

            if (!Guid.TryParse(request.PatientId, out var patientId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid patient ID format"));
            }

            var patient = await _patientService.GetPatientByIdAsync(patientId, context.CancellationToken);
            
            if (patient == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Patient not found"));
            }

            return new PatientResponse
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
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient {PatientId}", request.PatientId);
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }

    /// <summary>
    /// Creates a new patient
    /// </summary>
    public override async Task<CreatePatientResponse> CreatePatient(CreatePatientRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Creating new patient: {Email}", request.Email);

            if (!DateOnly.TryParse(request.DateOfBirth, out var dateOfBirth))
            {
                return new CreatePatientResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid date of birth format. Use yyyy-MM-dd."
                };
            }

            var dto = new CreatePatientDto(
                request.FirstName,
                request.LastName,
                request.Email,
                dateOfBirth
            );

            var patientId = await _patientService.CreatePatientAsync(dto, context.CancellationToken);

            return new CreatePatientResponse
            {
                PatientId = patientId.ToString(),
                Success = true
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating patient");
            return new CreatePatientResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            return new CreatePatientResponse
            {
                Success = false,
                ErrorMessage = "Internal server error"
            };
        }
    }

    /// <summary>
    /// Gets all patients
    /// </summary>
    public override async Task<GetAllPatientsResponse> GetAllPatients(GetAllPatientsRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Getting all patients");

            var patients = await _patientService.GetAllPatientsAsync(context.CancellationToken);
            
            var response = new GetAllPatientsResponse();
            
            foreach (var patient in patients)
            {
                response.Patients.Add(new PatientResponse
                {
                    PatientId = patient.Id.ToString(),
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    Email = patient.Email,
                    DateOfBirth = patient.DateOfBirth.ToString("yyyy-MM-dd"),
                    CreatedAt = patient.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    FullName = patient.FullName
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all patients");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
} 