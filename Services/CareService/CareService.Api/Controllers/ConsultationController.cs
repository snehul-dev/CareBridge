using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CareBridge.Shared.Responses;
using CareService.Application.DTOs.Consultations;
using CareService.Application.Interfaces;
using System.Net;
namespace CareService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;
        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }
        [HttpPost]
        [Authorize(Roles ="Doctor")]
        public async Task<ActionResult<ApiResponse<ConsultationDetailsResponseDto>>> Create(
            [FromBody] CreateConsultationRequestDto request,
            CancellationToken cancellationToken
            )
        {
            var doctorId = GetUserId();

            var result = await _consultationService.CreateAsync(request, doctorId, cancellationToken);
            var response = ApiResponse<ConsultationDetailsResponseDto>
                .Success(result,
                "Consultation created successfully",
                HttpStatusCode.Created
                );

            return CreatedAtAction(
              nameof(GetById),
              new { id = result.Id },
              response);
        }

        [HttpPatch("{id:guid}/start")]
        [Authorize(Roles ="Doctor")]
        public async Task<ActionResult<ApiResponse<ConsultationDetailsResponseDto>>> Start(Guid id , 
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var result = await _consultationService.StartAsync(id, doctorId, cancellationToken);

            var response = ApiResponse<ConsultationDetailsResponseDto>
                .Success(result,
                "Consultation started successfully"


                );

            return Ok(response);
        }

        [HttpPatch("{id:guid}/complete")]
        [Authorize(Roles ="Doctor")]
        public async Task<ActionResult<ApiResponse<ConsultationDetailsResponseDto>>> Complete(
            Guid id,
            CancellationToken cancellationToken
            )
        {
            var doctorId = GetUserId();

            var result = await _consultationService.CompleteAsync(id, doctorId, cancellationToken);

            var response = ApiResponse<ConsultationDetailsResponseDto>
                .Success(result,
                "Consultation Completed successfully"
                );
            return Ok(response);


        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles ="Doctor,Patient")]

        public async Task<ActionResult<ApiResponse <ConsultationDetailsResponseDto>>> GetById(
            Guid id,
            CancellationToken cancellationToken
            )
        {
            var userId = GetUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role))
                return Forbid();

            var result = await _consultationService.GetByIdAsync(id, userId,role, cancellationToken);

            var response = ApiResponse<ConsultationDetailsResponseDto>
                .Success(result,
                "Consultation retrieved successfully");

            return Ok(response);

        }

        [HttpGet("my")]
        [Authorize(Roles ="Doctor,Patient")]

        public async Task<ActionResult<ApiResponse<IEnumerable<ConsultationDetailsResponseDto>>>>
            GetMyConsultations(CancellationToken cancellationToken
            )
        {
            var userId = GetUserId();

            if (User.IsInRole("Doctor"))
            {
                var result = await _consultationService.GetDoctorConsultationsAsync(userId, cancellationToken);

                var response = ApiResponse<IEnumerable<ConsultationDetailsResponseDto>>
                    .Success(result,
                    "Doctor consultations retrieved successfully"
                    );
                return Ok(response);
            }

            var PatientResult = await _consultationService.GetPatientConsultaionAsync(userId, cancellationToken);
            var PatientResponse = ApiResponse<IEnumerable<ConsultationDetailsResponseDto>>
                .Success(PatientResult,
                "Patient consultations retrieved successfully");

            return StatusCode(PatientResponse.StatusCode, PatientResponse);

        }

        [HttpGet("patient/{patientId:guid}")]
        [Authorize(Roles ="Doctor,Admin")]
        public async Task<ActionResult<IEnumerable<ConsultationDetailsResponseDto>>> GetPatientConsultations(Guid patientId ,
            CancellationToken  cancellationToken)
        {
            if (User.IsInRole("Admin"))
            {
                var consultations = await _consultationService.GetPatientConsultaionAsync(patientId, cancellationToken);
                return Ok(consultations);
            }

            var doctorId = GetUserId();

            var doctorConsultations = await _consultationService.GetPatientConsultationsForDoctorAsync(patientId, doctorId, cancellationToken);
            return Ok(doctorConsultations);
        }





    private Guid GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            return Guid.Parse(userId);
        }
    }
}
