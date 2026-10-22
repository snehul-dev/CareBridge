using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppointmentService.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DoctorsController:ControllerBase
    {
        private readonly IUserServiceClient _userServiceClient;
        public DoctorsController(IUserServiceClient userServiceClient)
        {
            _userServiceClient = userServiceClient;
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

            var response = ApiResponse<IReadOnlyList<DoctorSearchResponse>>
                .Success(doctors,
                "Doctors retrieved successfully.",
                    HttpStatusCode.OK
                );

            return StatusCode(response.StatusCode, response);

            
        }

        [HttpGet("{doctorId:guid}")]
        public async Task<ActionResult<DoctorSearchResponse>> GetDoctorById(
                Guid doctorId,
                CancellationToken cancellationToken)
             {
            var doctor = await _userServiceClient.GetDoctorByIdAsync(
                doctorId,
                cancellationToken);

            var response = ApiResponse<DoctorSearchResponse>
                .Success(doctor,
                "Doctor retrieved successfully.",
                HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }
    }
}
