using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserService.Application.DTOs.Labs;
using UserService.Application.Interfaces;
using CareBridge.Contracts.Labs;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabsController:ControllerBase
    {

        private readonly ILabService _labService;
        public LabsController(ILabService labService)
        {
            _labService = labService;
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LabResponseDto>>>> GetVerifiedLabs(
            CancellationToken cancellationToken)
        {
            var labs = await _labService.GetVerifiedLabsAsync(cancellationToken);

            var response = ApiResponse<IEnumerable<LabResponseDto>>
                .Success(labs,
                "Verified Labs retrieved",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("{labId:guid}")]
        [Authorize(Roles ="Patient")]
        public async Task<ActionResult<ApiResponse<LabResponseDto>>> GetVerifiedLabById(
            Guid labId , CancellationToken cancellationToken
            )
        {
            var lab = await _labService.GetVerifiedLabByIdAsync(labId, cancellationToken);
            var response = ApiResponse<LabResponseDto>
                .Success(lab,
                "Verified Lab retrieved",
                HttpStatusCode.OK);

            return Ok(response);
        }

        [HttpGet("{labId:guid}/verification")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ApiResponse<LabVerificationResponseDto>>>CheckLabVerification(
        Guid labId,
        CancellationToken cancellationToken)
        {
            var result = await _labService.CheckLabVerificationAsync(
                labId,
                cancellationToken);

            var response = ApiResponse<LabVerificationResponseDto>
                .Success(
                    result,
                    "Lab verification status retrieved successfully",
                    HttpStatusCode.OK);

            return Ok(response);
        }
    }
}
