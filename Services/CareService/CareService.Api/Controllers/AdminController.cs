using CareBridge.Shared.Responses;
using CareService.Application.DTOs.Admin;
using CareService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CareService.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController:ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<
            ApiResponse<AdminDashboardResponseDto>>> GetDashboard(
                CancellationToken cancellationToken)
        {
            var dashboard = await _adminService.GetDashboardAsync(
                cancellationToken);

            var response =
                ApiResponse<AdminDashboardResponseDto>.Success(
                    dashboard,
                    "Admin dashboard retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/admin/consultations?from=2026-11-01&to=2026-11-30
        [HttpGet("consultations")]
        public async Task<ActionResult<
            ApiResponse<ConsultationStatisticsDto>>> GetConsultationStatistics(
                [FromQuery] DateTimeOffset? from,
                [FromQuery] DateTimeOffset? to,
                CancellationToken cancellationToken)
        {
            var statistics =
                await _adminService.GetConsultationStatisticsAsync(
                    from,
                    to,
                    cancellationToken);

            var response =
                ApiResponse<ConsultationStatisticsDto>.Success(
                    statistics,
                    "Consultation statistics retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/admin/lab-orders?status=Pending
        [HttpGet("lab-orders")]
        public async Task<ActionResult<
            ApiResponse<LabOrderStatisticsDto>>> GetLabOrderStatistics(
                [FromQuery] string? status,
                CancellationToken cancellationToken)
        {
            var statistics =
                await _adminService.GetLabOrderStatisticsAsync(
                    status,
                    cancellationToken);

            var response =
                ApiResponse<LabOrderStatisticsDto>.Success(
                    statistics,
                    "Lab order statistics retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/admin/stats/counts
        [HttpGet("stats/counts")]
        public async Task<ActionResult<
            ApiResponse<EntityCountsDto>>> GetEntityCounts(
                CancellationToken cancellationToken)
        {
            var counts = await _adminService.GetEntityCountsAsync(
                cancellationToken);

            var response = ApiResponse<EntityCountsDto>.Success(
                counts,
                "Platform counts retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/admin/appointments/stats
        [HttpGet("appointments/stats")]
        public async Task<ActionResult<
            ApiResponse<AppointmentStatisticsDto>>> GetAppointmentStatistics(
                CancellationToken cancellationToken)
        {
            var statistics = await _adminService
                .GetAppointmentStatisticsAsync(cancellationToken);

            var response =
                ApiResponse<AppointmentStatisticsDto>.Success(
                    statistics,
                    "Appointment statistics retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/admin/appointments/trends?year=2026
        [HttpGet("appointments/trends")]
        public async Task<ActionResult<
            ApiResponse<IReadOnlyList<AppointmentTrendDto>>>> GetAppointmentTrends(
                [FromQuery] int? year,
                CancellationToken cancellationToken)
        {
            var trends = await _adminService.GetAppointmentTrendsAsync(
                year,
                cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<AppointmentTrendDto>>.Success(
                    trends,
                    "Appointment trends retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/admin/doctors/popular?limit=5
        [HttpGet("doctors/popular")]
        public async Task<ActionResult<
            ApiResponse<IReadOnlyList<PopularDoctorDto>>>> GetPopularDoctors(
                [FromQuery] int limit = 5,
                CancellationToken cancellationToken = default)
        {
            var doctors = await _adminService.GetPopularDoctorsAsync(
                limit,
                cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<PopularDoctorDto>>.Success(
                    doctors,
                    "Popular doctors retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }
    }
}
