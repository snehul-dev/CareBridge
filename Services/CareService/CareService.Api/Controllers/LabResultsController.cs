using CareBridge.Shared.Responses;
using CareService.Application.DTOs.LabOrders;
using CareService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CareService.Api.Controllers
{
    [ApiController]
    [Route("api/lab-results")]
    [Authorize]
    public class LabResultsController:ControllerBase
    {
        private readonly ILabOrderService _labOrderService;
        private readonly ILabResultPdfService _labResultPdfService;
        public LabResultsController(ILabOrderService labOrderService , 
            ILabResultPdfService labResultPdfService)
        {
            _labOrderService = labOrderService;
            _labResultPdfService = labResultPdfService;
        }

        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ApiResponse<List<LabResultResponseDto>>>>GetMyResults(CancellationToken cancellationToken)
        {
            var patientId = GetUserId();

            var orders = await _labOrderService.GetPatientOrdersAsync(
                patientId,
                cancellationToken);

            var allResults = new List<LabResultResponseDto>();

            foreach (var order in orders)
            {
                var results = await _labOrderService.GetResultsAsync(
                    order.Id,
                    patientId,
                    "patient",
                    cancellationToken);

                allResults.AddRange(results);
            }

            var response = ApiResponse<List<LabResultResponseDto>>.Success(
                allResults,
                "Lab results retrieved successfully",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("order/{labOrderId:guid}")]
        [Authorize(Roles = "Patient,Doctor,Lab")]
        public async Task<ActionResult<ApiResponse<List<LabResultResponseDto>>>> GetResultsByOrder(
            Guid labOrderId,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new UnauthorizedAccessException(
                    "User role not found.");
            }

            var results = await _labOrderService.GetResultsAsync(
                labOrderId,
                userId,
                role.ToLowerInvariant(),
                cancellationToken);

            var response = ApiResponse<List<LabResultResponseDto>>
                .Success(
                results,
                "Lab results retrieved successfully",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("doctor/my")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ApiResponse<List<LabResultResponseDto>>>>GetDoctorResults(
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var orders = await _labOrderService.GetDoctorOrdersAsync(
                doctorId,
                cancellationToken);

            var allResults = new List<LabResultResponseDto>();

            foreach (var order in orders)
            {
                var results = await _labOrderService.GetResultsAsync(
                    order.Id,
                    doctorId,
                    "doctor",
                    cancellationToken);

                allResults.AddRange(results);
            }

            var response = ApiResponse<List<LabResultResponseDto>>.Success(
                allResults,
                "Doctor lab results retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpPost("{id:guid}/upload")]
        [Authorize(Roles = "Lab")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(12 * 1024 * 1024)]
        public async Task<IActionResult> UploadLabReport(
                Guid id,
                IFormFile file,
                CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(
                    ApiResponse<string>.Fail(
                        new[] { "Please select a PDF file." },
                        "File upload failed.",
                        HttpStatusCode.BadRequest));
            }

            var labId = GetLabId();

            await using var stream = file.OpenReadStream();

            await _labResultPdfService.UploadAsync(
                id,
                labId,
                stream,
                file.FileName,
                cancellationToken);

            var response = ApiResponse<string>.Success(
                "Lab report uploaded successfully.",
                "PDF uploaded successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }
        [HttpGet("{id:guid}/download")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> DownloadLabReport(
             Guid id,
             CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(role))
                return Forbid();

            var pdf = await _labResultPdfService.DownloadAsync(
                id,
                userId,
                role,
                cancellationToken);

            return File(
                pdf.Content,
                "application/pdf",
                pdf.FileName);
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated or user ID is invalid.");
            }

            return parsedUserId;
        }

        private Guid GetLabId()
        {
            var labId = User.FindFirstValue("lab_id");

            if (!Guid.TryParse(labId, out var parsedLabId))
            {
                throw new UnauthorizedAccessException(
                    "Lab profile ID not found in JWT.");
            }

            return parsedLabId;
        }
    }
}
