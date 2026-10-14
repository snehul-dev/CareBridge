using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserService.Api.Controllers
{
    [ApiController]
    public class BaseController:ControllerBase
    {
        protected Guid GetUserId()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated or user ID is invalid."
                );
            }

            return parsedUserId;
        }
    }
}
