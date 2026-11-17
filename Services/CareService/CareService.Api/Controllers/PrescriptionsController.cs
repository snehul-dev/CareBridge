using CareBridge.Shared.Responses;
using CareService.Application.DTOs.Prescriptions;
using CareService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CareService.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PrescriptionsController:ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPrescriptionPdfService _prescriptionPdfService;

        public PrescriptionsController(IPrescriptionService prescriptionService,
            IPrescriptionPdfService prescriptionPdfService)
        {
            _prescriptionService = prescriptionService;
            _prescriptionPdfService = prescriptionPdfService;
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ApiResponse<PrescriptionResponseDto>>> Create(
            CreatePrescriptionRequestDto request,
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var result = await _prescriptionService.CreateAsync(
               request,
               doctorId,
               cancellationToken);

            var response = ApiResponse<PrescriptionResponseDto>
                .Success(
                result,
                "Prescription created successfully.",
                HttpStatusCode.Created
                );

            return CreatedAtAction(
           nameof(GetById),
           new { id = result.Id },
           response);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<PrescriptionResponseDto>>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var result = await _prescriptionService.GetByIdAsync(
                id,
                userId,
                role,
                cancellationToken);

            var response = ApiResponse<PrescriptionResponseDto>.Success(
                result,
                "Prescription retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PrescriptionResponseDto>>>> GetMyPrescriptions(

            CancellationToken cancellationToken)
        {
            var patientId = GetUserId();

            var result = await _prescriptionService.GetMyPrescriptionsAsync(
                patientId,
                cancellationToken);

            var response = ApiResponse<IEnumerable<PrescriptionResponseDto>>.Success(
                result,
                "Your prescriptions retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        
        [HttpGet("consultation/{consultationId:guid}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<PrescriptionResponseDto>>> GetByConsultationId(
            Guid consultationId,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var result = await _prescriptionService.GetByConsultationIdAsync(
                consultationId,
                userId,
                role,
                cancellationToken);

            var response = ApiResponse<PrescriptionResponseDto>.Success(
                result,
                "Prescription retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("{id:guid}/pdf")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> DownloadPdf(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var pdfBytes = await _prescriptionPdfService.GetPdfAsync(
                id,
                userId,
                role,
                cancellationToken);

            return File(pdfBytes, "application/pdf", $"prescription-{id}.pdf");
        }

        [HttpPost("{id:guid}/pdf")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ApiResponse<PrescriptionResponseDto>>> GeneratePdf(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            await _prescriptionPdfService.GenerateAndStoreAsync(
                id,
                userId,
                role,
                cancellationToken);

            var result = await _prescriptionService.GetByIdAsync(
                id,
                userId,
                role,
                cancellationToken);

            var response = ApiResponse<PrescriptionResponseDto>.Success(
                result,
                "Prescription PDF generated successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }



        private Guid GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!Guid.TryParse(userId , out var id))
            {
                throw new UnauthorizedAccessException(
                     "Invalid user ID in authentication token.");
            }
            return id;
        }

        private string GetUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role)
                ?? throw new UnauthorizedAccessException(
                    "User role not found in authentication token.");
        }


    }
}
