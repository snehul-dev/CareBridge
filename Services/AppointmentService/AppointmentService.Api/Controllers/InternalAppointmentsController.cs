using AppointmentService.Application.DTOs.Admin;
using AppointmentService.Application.Interfaces;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppointmentService.Api.Controllers
{
    [ApiController]
    [Route("api/appointments/internal/appointments")]
    [Authorize(Policy = "CareServiceOnly")]
    public class InternalAppointmentsController : ControllerBase
    {
        private readonly IAppointmentStatsRepository _statsRepository;

        public InternalAppointmentsController(
            IAppointmentStatsRepository statsRepository)
        {
            _statsRepository = statsRepository;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<AppointmentStatisticsDto>>> GetStatistics(
            CancellationToken cancellationToken)
        {
            var statistics = await _statsRepository
                .GetStatisticsAsync(cancellationToken);

            var response =
                ApiResponse<AppointmentStatisticsDto>.Success(
                    statistics,
                    "Appointment statistics retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/internal/appointments/trends?year=2026
        [HttpGet("trends")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentTrendDto>>>> GetMonthlyTrends(
            [FromQuery] int? year,
            CancellationToken cancellationToken)
        {
            var trends = await _statsRepository
                .GetMonthlyTrendsAsync(
                    year ?? DateTime.UtcNow.Year,
                    cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<AppointmentTrendDto>>.Success(
                    trends,
                    "Appointment trends retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/internal/appointments/popular-doctors?limit=5
        [HttpGet("popular-doctors")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PopularDoctorAppointmentDto>>>> GetPopularDoctors(
            [FromQuery] int limit = 5,
            CancellationToken cancellationToken = default)
        {
            if (limit < 1 || limit > 50)
            {
                limit = 5;
            }

            var doctors = await _statsRepository
                .GetPopularDoctorsAsync(limit, cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<PopularDoctorAppointmentDto>>.Success(
                    doctors,
                    "Popular doctors retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }
    }
}
