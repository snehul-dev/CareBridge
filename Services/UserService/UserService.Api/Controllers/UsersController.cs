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
        public async Task<IActionResult> GetMyProfileAsync()
        {
            var userId = GetUserId();

            var profile = await _userProfileService.GetMyProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateMyProfileRequest request)
        {
            var userId = GetUserId();
            await _userProfileService.UpdateMyProfileAsync(userId, request);

            return NoContent();

        }
    }
}
