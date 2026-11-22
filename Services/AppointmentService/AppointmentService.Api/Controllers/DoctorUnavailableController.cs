using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AppointmentService.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Doctor")]
    [Route("api/appointments/doctors/unavailable")]
    public class DoctorUnavailableController : ControllerBase
    {
        private readonly IDoctorUnavailablePeriodService _unavailablePeriodService;

        public DoctorUnavailableController(
            IDoctorUnavailablePeriodService unavailablePeriodService)
        {
            _unavailablePeriodService = unavailablePeriodService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<DoctorUnavailableResponseDto>>> CreateUnavailablePeriod(
            [FromBody] DoctorUnavailableRequestDto request,
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var period = await _unavailablePeriodService.CreateAsync(
                doctorId,
                request,
                cancellationToken);

            var message = period.CancelledAppointmentsCount > 0
                ? $"Unavailable period created successfully. {period.CancelledAppointmentsCount} existing appointment(s) have been cancelled."
                : "Unavailable period created successfully.";

            var response = ApiResponse<DoctorUnavailableResponseDto>.Success(
                period,
                message,
                HttpStatusCode.Created);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorUnavailableResponseDto>>>> GetMyUnavailablePeriods(
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var periods = await _unavailablePeriodService.GetMyUnavailablePeriodsAsync(
                doctorId,
                cancellationToken);

            var response = ApiResponse<IReadOnlyList<DoctorUnavailableResponseDto>>.Success(
                periods,
                "Your unavailable periods retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUnavailablePeriod(
            Guid id,
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var restoredCount = await _unavailablePeriodService.DeleteAsync(
                id,
                doctorId,
                cancellationToken);

            var message = restoredCount > 0
                ? $"Unavailable period deleted successfully. {restoredCount} cancelled appointment(s) have been restored."
                : "Unavailable period deleted successfully.";

            var response = ApiResponse<object>.Success(
                null,
                message,
                HttpStatusCode.OK);

            return Ok(response);
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
