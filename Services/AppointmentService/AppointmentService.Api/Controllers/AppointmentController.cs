using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace AppointmentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController:ControllerBase
    {

        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [HttpGet("doctors/{doctorId:guid}/availability")]
        public async Task<ActionResult<IReadOnlyList<AvailableAppointmentSlotDto>>> GetAvailableSlots(
              Guid doctorId,
              [FromQuery] DateOnly date,
              CancellationToken cancellationToken)
        {
            var slot = await _appointmentService.GetAvailableSlotsAsync(doctorId,
            date,
            cancellationToken);

            var response = ApiResponse<IReadOnlyList<AvailableAppointmentSlotDto>>
                .Success(slot, 
                "Available appointment slots retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ApiResponse<AppointmentResponseDto>>>
        BookAppointment(
        [FromBody] BookAppointmentRequestDto request,
        CancellationToken cancellationToken)
        {
            var patientIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(patientIdClaim, out var patientId))
            {
                var unauthorizedResponse =
                    ApiResponse<AppointmentResponseDto>.Fail(
                        new[]
                        {
                    "User is not authenticated with a valid patient ID."
                        },
                        "Unauthorized",
                        HttpStatusCode.Unauthorized);

                return Unauthorized(unauthorizedResponse);
            }

            var appointment = await _appointmentService.BookAppointmentAsync(
                request,
                patientId,
                cancellationToken);

            var response = ApiResponse<AppointmentResponseDto>.Success(
                appointment,
                "Appointment booked successfully.",
                HttpStatusCode.Created);

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
