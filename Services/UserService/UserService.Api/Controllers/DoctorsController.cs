using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Interfaces;
using CareBridge.Shared.Responses;
using UserService.Application.DTOs.Doctors;
using System.Net;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? specialization,
            [FromQuery] string? city,
            [FromQuery] string? district,
            CancellationToken cancellationToken)
        {
            var doctors = await _doctorService.SearchAsync(
                specialization,
                city,
                district,
                cancellationToken);
            Console.WriteLine(doctors);

            var response = ApiResponse<IReadOnlyList<DoctorSearchResponse>>
                .Success(
                    doctors,
                    "Doctors retrieved successfully.",
                    HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{doctorId:guid}")]
        public async Task<IActionResult> GetById(Guid doctorId , CancellationToken cancellationToken)
        {
            var doctor = await _doctorService.GetByIdAsync(doctorId, cancellationToken);

            var response = ApiResponse<DoctorSearchResponse>
                .Success(doctor,
                "Doctor retrieved successfully.",
                HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);

        }
    }
}