
using CareBridge.Shared.Responses;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public UsersController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
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
    }
}
