using CareBridge.Contracts.Users;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using UserService.Application.DTOs.Admin;
using UserService.Application.Interfaces;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/users/internal/users")]
    [Authorize(Policy = "CareServiceOnly")]
    public class InternalUsersController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAdminRepository _adminRepository;

        public InternalUsersController(
            IUserProfileService userProfileService,
            IAdminRepository adminRepository)
        {
            _userProfileService = userProfileService;
            _adminRepository = adminRepository;
        }

        [HttpGet("counts")]
        public async Task<ActionResult<ApiResponse<RoleCountsDto>>> GetRoleCounts(
            CancellationToken cancellationToken)
        {
            var counts = await _adminRepository
                .GetRoleCountsAsync(cancellationToken);

            var response = ApiResponse<RoleCountsDto>.Success(
                counts,
                "Role counts retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }

        // GET: api/internal/users/doctor-summaries?ids=...&ids=...
        [HttpGet("doctor-summaries")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorSummaryDto>>>> GetDoctorSummaries(
            [FromQuery] Guid[] ids,
            CancellationToken cancellationToken)
        {
            var summaries = await _adminRepository
                .GetDoctorSummariesAsync(ids ?? Array.Empty<Guid>(), cancellationToken);

            var response =
                ApiResponse<IReadOnlyList<DoctorSummaryDto>>.Success(
                    summaries,
                    "Doctor summaries retrieved successfully.",
                    HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("prescription-profile")]
        public async Task<IActionResult> GetPrescriptionProfile(
            [FromQuery] Guid doctorUserId,
            [FromQuery] Guid patientUserId,
            CancellationToken cancellationToken)
        {
            var profile = await _userProfileService
                .GetInternalPrescriptionProfileAsync(
                    doctorUserId,
                    patientUserId,
                    cancellationToken);

            var response =
                ApiResponse<PrescriptionProfileResponseDto>.Success(
                    profile,
                    "Prescription profile retrieved successfully.",
                    HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }
    }
}
