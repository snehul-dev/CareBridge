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
        public LabResultsController(ILabOrderService labOrderService)
        {
            _labOrderService = labOrderService;
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
    }
}
