using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserService.Application.Interfaces;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/users/internal/labs")]
    [Authorize(Policy = "CareServiceOnly")]
    public class InternalLabsController : ControllerBase
    {
        private readonly ILabRepository _labRepository;

        public InternalLabsController(ILabRepository labRepository)
        {
            _labRepository = labRepository;
        }

        [HttpGet("{labId:guid}")]
        public async Task<IActionResult> GetLab(
            Guid labId,
            CancellationToken cancellationToken)
        {
            var lab = await _labRepository.GetByIdAsync(labId, cancellationToken);

            if (lab == null)
                return NotFound(
                    ApiResponse<string>.Fail(
                        new[] { "Lab not found." },
                        "Lab not found.",
                        HttpStatusCode.NotFound));

            var data = new
            {
                lab.Id,
                lab.LabName,
                lab.UserId
            };

            var response = ApiResponse<object>.Success(
                data,
                "Lab retrieved successfully.",
                HttpStatusCode.OK);

            return Ok(response);
        }
    }
}
