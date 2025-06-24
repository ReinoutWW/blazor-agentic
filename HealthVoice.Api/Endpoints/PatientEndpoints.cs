using Microsoft.AspNetCore.Mvc;
using HealthVoice.Business.Services;
using HealthVoice.Business.DTOs;
using HealthVoice.Api.Mappers;

namespace HealthVoice.Api.Endpoints;

/// <summary>
/// Extension methods for configuring Patient endpoints
/// </summary>
public static class PatientEndpoints
{
    /// <summary>
    /// Maps patient-related endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapPatientEndpoints(this WebApplication app)
    {
        var patients = app.MapGroup("/api/v{version:apiVersion}/patients")
            .WithTags("Patients")
            .WithOpenApi();

        // GET /api/v1/patients
        patients.MapGet("/", GetAllPatients)
            .WithName("GetAllPatients")
            .WithSummary("Get all patients")
            .WithDescription("Retrieves all patients from the system")
            .Produces<IEnumerable<PatientResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/v1/patients/{id}
        patients.MapGet("/{id:guid}", GetPatientById)
            .WithName("GetPatientById")
            .WithSummary("Get patient by ID")
            .WithDescription("Retrieves a specific patient by their unique identifier")
            .Produces<PatientResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        // POST /api/v1/patients
        patients.MapPost("/", CreatePatient)
            .WithName("CreatePatient")
            .WithSummary("Create a new patient")
            .WithDescription("Creates a new patient in the system")
            .Accepts<CreatePatientRequest>("application/json")
            .Produces<CreatePatientResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }

    /// <summary>
    /// Gets all patients
    /// </summary>
    internal static async Task<IResult> GetAllPatients(
        PatientService patientService,
        CancellationToken cancellationToken)
    {
        try
        {
            var patients = await patientService.GetAllPatientsAsync(cancellationToken);
            var response = patients.Select(p => p.ToRestResponse());

            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while retrieving patients",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Gets a patient by ID
    /// </summary>
    internal static async Task<IResult> GetPatientById(
        [FromRoute] Guid id,
        PatientService patientService,
        CancellationToken cancellationToken)
    {
        try
        {
            var patient = await patientService.GetPatientByIdAsync(id, cancellationToken);

            if (patient == null)
            {
                return Results.NotFound(new { Message = $"Patient with ID {id} not found" });
            }

            var response = patient.ToRestResponse();

            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while retrieving the patient",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Creates a new patient
    /// </summary>
    internal static async Task<IResult> CreatePatient(
        [FromBody] CreatePatientRequest request,
        PatientService patientService,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = new CreatePatientDto(
                request.FirstName,
                request.LastName,
                request.Email,
                request.DateOfBirth
            );

            var patientId = await patientService.CreatePatientAsync(dto, cancellationToken);

            var response = new CreatePatientResponse
            {
                Id = patientId,
                Message = "Patient created successfully"
            };

            return Results.Created($"/api/v1/patients/{patientId}", response);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { Message = ex.Message });
        }
        catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while creating the patient",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }
}

/// <summary>
/// Patient response model for REST API
/// </summary>
public class PatientResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FullName { get; set; } = string.Empty;
}

/// <summary>
/// Create patient request model for REST API
/// </summary>
public class CreatePatientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
}

/// <summary>
/// Create patient response model for REST API
/// </summary>
public class CreatePatientResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
}