using CareBridge.Shared.Responses;
using CareService.Application.DTOs.LabOrders;
using CareService.Application.Interfaces;
using CareService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CareService.Api.Controllers
{
    [ApiController]
    [Route("api/care/lab-results")]
    [Authorize]
    public class LabResultsController:ControllerBase
    {
        private readonly ILabOrderService _labOrderService;
        private readonly ILabResultPdfService _labResultPdfService;
        private readonly IUserProfileClient _userProfileClient;
        private readonly IInternalLabClient _internalLabClient;
        public LabResultsController(ILabOrderService labOrderService , 
            ILabResultPdfService labResultPdfService,
            IUserProfileClient userProfileClient,
            IInternalLabClient internalLabClient)
        {
            _labOrderService = labOrderService;
            _labResultPdfService = labResultPdfService;
            _userProfileClient = userProfileClient;
            _internalLabClient = internalLabClient;
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
            var labNameCache = new Dictionary<Guid, string?>();

            foreach (var order in orders)
            {
                var results = await _labOrderService.GetResultsAsync(
                    order.Id,
                    patientId,
                    "patient",
                    cancellationToken);

                await EnrichResultsAsync(
                    results,
                    order,
                    labNameCache,
                    cancellationToken);

                allResults.AddRange(results);
            }

            var response = ApiResponse<List<LabResultResponseDto>>.Success(
                allResults,
                "Lab results retrieved successfully",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("lab/my")]
        [Authorize(Roles = "Lab")]
        public async Task<ActionResult<ApiResponse<List<LabResultResponseDto>>>>GetLabResults(
            [FromQuery] string? username,
            [FromQuery] string? testName,
            CancellationToken cancellationToken)
        {
            var labId = GetLabId();

            var orders = await _labOrderService.GetLabOrdersAsync(
                labId,
                cancellationToken);

            var allResults = new List<LabResultResponseDto>();
            var patientNameCache = new Dictionary<Guid, (string? Name, string? Email)>();
            var uploaderNameCache = new Dictionary<Guid, string?>();

            foreach (var order in orders)
            {
                var results = await _labOrderService.GetResultsAsync(
                    order.Id,
                    labId,
                    "lab",
                    cancellationToken);

                // Enrich with patient info
                var cacheKey = order.PatientId;
                if (!patientNameCache.TryGetValue(cacheKey, out var patientInfo))
                {
                    patientInfo = await FetchPatientInfoAsync(order.DoctorId, order.PatientId, cancellationToken);
                    patientNameCache[cacheKey] = patientInfo;
                }

                foreach (var result in results)
                {
                    result.PatientName = patientInfo.Name;
                    result.Username = patientInfo.Email;
                    result.PatientEmail = patientInfo.Email;

                    // Resolve uploader (lab) name
                    if (result.UploadedBy.HasValue)
                    {
                        var uploaderId = result.UploadedBy.Value;
                        if (!uploaderNameCache.TryGetValue(uploaderId, out var uploaderName))
                        {
                            uploaderName = await SafeGetLabNameAsync(uploaderId, cancellationToken);
                            uploaderNameCache[uploaderId] = uploaderName;
                        }
                        result.UploadedByName = uploaderName;
                    }
                }

                allResults.AddRange(results);
            }

            // Apply filters
            if (!string.IsNullOrWhiteSpace(username))
            {
                allResults = allResults
                    .Where(r => r.PatientName?.Contains(username, StringComparison.OrdinalIgnoreCase) == true ||
                               r.Username?.Contains(username, StringComparison.OrdinalIgnoreCase) == true ||
                               r.PatientEmail?.Contains(username, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(testName))
            {
                allResults = allResults
                    .Where(r => r.TestName?.Contains(testName, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }

            var response = ApiResponse<List<LabResultResponseDto>>.Success(
                allResults,
                "Lab results retrieved successfully",
                HttpStatusCode.OK);

            return Ok(response);
        }

        private async Task<(string? Name, string? Email)> FetchPatientInfoAsync(
            Guid doctorUserId,
            Guid patientUserId,
            CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _userProfileClient
                    .GetPrescriptionProfileAsync(
                        doctorUserId,
                        patientUserId,
                        cancellationToken);

                if (profile?.Patient == null)
                    return (null, null);

                var name = $"{profile.Patient.FirstName} {profile.Patient.LastName}".Trim();
                var email = profile.Patient.Email;

                return (
                    string.IsNullOrWhiteSpace(name) ? null : name,
                    string.IsNullOrWhiteSpace(email) ? null : email);
            }
            catch
            {
                return (null, null);
            }
        }

        private async Task<string?> SafeGetLabNameAsync(
            Guid labId,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _internalLabClient.GetLabNameAsync(labId, cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        private async Task EnrichResultsAsync(
            List<LabResultResponseDto> results,
            LabOrderResponseDto order,
            Dictionary<Guid, string?> labNameCache,
            CancellationToken cancellationToken)
        {
            string? labName = null;

            if (order.AssignedLabId.HasValue)
            {
                var labId = order.AssignedLabId.Value;

                if (!labNameCache.TryGetValue(labId, out labName))
                {
                    labName = await SafeGetLabNameAsync(labId, cancellationToken);
                    labNameCache[labId] = labName;
                }
            }

            var patientName = order.PatientName;

            if (string.IsNullOrWhiteSpace(patientName))
            {
                var info = await FetchPatientInfoAsync(
                    order.DoctorId,
                    order.PatientId,
                    cancellationToken);

                patientName = info.Name;
            }

            foreach (var result in results)
            {
                result.LabName = labName;
                result.PatientName = patientName;
                result.DoctorId = order.DoctorId;
                result.DoctorName = order.DoctorName;
                result.DoctorSpecialization = order.DoctorSpecialization;
            }
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

            var order = await _labOrderService.GetByIdAsync(
                labOrderId,
                userId,
                role.ToLowerInvariant(),
                cancellationToken);

            await EnrichResultsAsync(
                results,
                order,
                new Dictionary<Guid, string?>(),
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
            var labNameCache = new Dictionary<Guid, string?>();

            foreach (var order in orders)
            {
                var results = await _labOrderService.GetResultsAsync(
                    order.Id,
                    doctorId,
                    "doctor",
                    cancellationToken);

                await EnrichResultsAsync(
                    results,
                    order,
                    labNameCache,
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
        [Authorize(Roles = "Doctor,Patient,Lab")]
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
