
using CareBridge.Contracts.Users;
using CareBridge.Shared.Exceptions;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using UserService.Application.DTOs.Users;
using UserService.Application.Interfaces;
using UserService.Application.Services;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController:BaseController
    {

        private readonly IUserProfileService _userProfileService;
        private readonly IProfilePictureService _profilePictureService;

        public UsersController(
            IUserProfileService userProfileService,
            IProfilePictureService profilePictureService)
        {
            _userProfileService = userProfileService;
            _profilePictureService = profilePictureService;
        }


        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfileAsync(CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var profile = await _userProfileService.GetMyProfileAsync(userId, cancellationToken);
            var response = ApiResponse<object>.Success(profile, "Profile retrieved successfully.");

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateMyProfileRequest request ,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            await _userProfileService.UpdateMyProfileAsync(userId, request , cancellationToken);

            var response = ApiResponse<object>.Success(null, "Profile updated successfully.");

            return StatusCode(response.StatusCode, response);

         

        }

        [HttpPost("me/profile-picture")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadProfilePictureAsync(
            IFormFile? file,
            CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                throw new BadRequestException(
                    "Please select an image to upload.");

            using var buffer = new MemoryStream();
            await file.CopyToAsync(buffer, cancellationToken);

            var pictureUrl = await _profilePictureService
                .UploadProfilePictureAsync(
                    GetUserId(),
                    buffer.ToArray(),
                    cancellationToken);

            var response = ApiResponse<string>.Success(
                pictureUrl,
                "Profile picture updated successfully.");

            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet("{userId:guid}/profile-picture")]
        public async Task<IActionResult> GetProfilePictureAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var picture = await _profilePictureService
                .GetProfilePictureAsync(userId, cancellationToken);

            Response.Headers.CacheControl = "public, max-age=604800";

            return File(picture.Content, picture.ContentType);
        }

        [HttpGet("summaries")]
        public async Task<IActionResult> GetUserSummariesAsync(
            [FromQuery] Guid[] ids,
            CancellationToken cancellationToken)
        {
            var summaries = await _userProfileService
                .GetUserSummariesAsync(
                    ids ?? Array.Empty<Guid>(),
                    cancellationToken);

            var response = ApiResponse<IReadOnlyList<UserSummaryDto>>
                .Success(
                    summaries,
                    "User summaries retrieved successfully.",
                    HttpStatusCode.OK);

            return StatusCode(response.StatusCode, response);
        }



    }
}
