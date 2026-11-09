using CareBridge.Contracts.Users;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using UserService.Application.Interfaces;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/internal/users")]
    [Authorize(Policy = "CareServiceOnly")]
    public class InternalUsersController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public InternalUsersController(
            IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
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
