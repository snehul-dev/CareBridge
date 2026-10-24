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
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
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
        public async Task<
            ActionResult<ApiResponse<AppointmentResponseDto>>>
            BookAppointment(
                [FromBody] BookAppointmentRequestDto request,
                CancellationToken cancellationToken)
        {
            var patientId = GetUserId();

            var appointment =
                await _appointmentService.BookAppointmentAsync(
                    request,
                    patientId,
                    cancellationToken);

            var response =
                ApiResponse<AppointmentResponseDto>.Success(
                    appointment,
                    "Appointment booked successfully.",
                    HttpStatusCode.Created);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<
                    ActionResult<ApiResponse<IReadOnlyList<AppointmentResponseDto>>>>
                    GetMyAppointments(
                        CancellationToken cancellationToken)
        {
            var patientId = GetUserId();

            var appointments =
                await _appointmentService.GetMyAppointmentsAsync(
                    patientId,
                    cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<AppointmentResponseDto>>.Success(
                    appointments,
                    "Your appointments retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("doctor/my")]
        [Authorize(Roles = "Doctor")]
        public async Task<
                        ActionResult<ApiResponse<IReadOnlyList<AppointmentResponseDto>>>>
                        GetDoctorAppointmentsAsync(
                        CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var appointments =
                await _appointmentService.GetDoctorAppointmentsAsync(doctorId, cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<AppointmentResponseDto>>
                .Success(
                    appointments,
                    "Your appointments retrieved successfully.",
                    HttpStatusCode.OK
                    );
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ApiResponse<AppointmentResponseDto>>> GetAppointmentById(Guid id , CancellationToken cancellationToken)
        {
            var patientId = GetUserId();
            var appointment = await _appointmentService.GetAppointmentById(id, patientId, cancellationToken);

            var response = ApiResponse<AppointmentResponseDto>
                .Success(
                appointment,
                "Your appointment retrieved successfully.",
                HttpStatusCode.OK
                );
            
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
