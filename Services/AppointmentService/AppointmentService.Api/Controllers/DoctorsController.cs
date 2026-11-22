
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
    [Authorize]
    [Route("api/appointments/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IUserServiceClient _userServiceClient;
        private readonly IDoctorAvailabilityService _availabilityService;

        public DoctorsController(
            IUserServiceClient userServiceClient,
            IDoctorAvailabilityService availabilityService)
        {
            _userServiceClient = userServiceClient;
            _availabilityService = availabilityService;
        }

        
        private Guid GetDoctorId()
        {
            var doctorId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(doctorId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "Invalid doctor identity.");
            }

            return id;
        }

      
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<DoctorSearchResponse>>> SearchDoctors(
                [FromQuery] string? specialization,
                [FromQuery] string? city,
                [FromQuery] string? district,
                CancellationToken cancellationToken)
        {
            var doctors = await _userServiceClient.SearchDoctorsAsync(
                specialization,
                city,
                district,
                cancellationToken);

            var response =ApiResponse<IReadOnlyList<DoctorSearchResponse>>.Success(
                    doctors,
                    "Doctors retrieved successfully.",
                    HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }

    
        [HttpGet("{doctorId:guid}")]
        public async Task<ActionResult<DoctorSearchResponse>>
            GetDoctorById(
                Guid doctorId,
                CancellationToken cancellationToken)
        {
            var doctor = await _userServiceClient.GetDoctorByIdAsync(
                doctorId,
                cancellationToken);

            var response = ApiResponse<DoctorSearchResponse>.Success(
                doctor,
                "Doctor retrieved successfully.",
                HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("availability/week")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddWeeklyAvailability(
            [FromBody] List<DoctorAvailabilityRequestDto> requests,
            CancellationToken cancellationToken)
        {
            var doctorId = GetDoctorId();

            var result = await _availabilityService.AddWeeklyAvailabilityAsync(
                    doctorId,
                    requests,
                    cancellationToken);

            var response = ApiResponse<IReadOnlyList<DoctorAvailabilityResponseDto>>.Success(
                    result,
                    "Doctor availability added successfully.",
                    HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("availability/week")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetWeeklyAvailability(
            CancellationToken cancellationToken)
        {
            var doctorId = GetDoctorId();

            var result =
                await _availabilityService.GetWeeklyAvailabilityAsync(
                    doctorId,
                    cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<DoctorAvailabilityResponseDto>>.Success(
                    result,
                    "Doctor availability retrieved successfully.",
                    HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }

   
        [HttpPut("availability/{id:guid}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateAvailability(
            Guid id,
            [FromBody] DoctorAvailabilityRequestDto request,
            CancellationToken cancellationToken)
        {
            var doctorId = GetDoctorId();

            var result =
                await _availabilityService.UpdateAvailabilityAsync(
                    doctorId,
                    id,
                    request,
                    cancellationToken);

            var response =
                ApiResponse<DoctorAvailabilityResponseDto>.Success(
                    result,
                    "Doctor availability updated successfully.",
                    HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }

  
        [HttpDelete("availability/{id:guid}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteAvailability(
            Guid id,
            CancellationToken cancellationToken)
        {
            var doctorId = GetDoctorId();

            await _availabilityService.DeleteAvailabilityAsync(
                doctorId,
                id,
                cancellationToken);

            var response = ApiResponse<object>.Success(
                new { },
                "Doctor availability deleted successfully.",
                HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }
    }
}
