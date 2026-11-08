
using CareBridge.Shared.Responses;
using CareService.Application.DTOs.LabOrders;
using CareService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CareService.Api.Controllers
{
    [ApiController]
    [Route("api/lab-orders")]
    [Authorize]
    public class LabOrderController : ControllerBase
    {
        private readonly ILabOrderService _labOrderService;

        public LabOrderController(ILabOrderService labOrderService)
        {
            _labOrderService = labOrderService;
        }

       
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateLabOrder(
            [FromBody] CreateLabOrderRequestDto request,
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var result = await _labOrderService.CreateAsync(
                request,
                doctorId,
                cancellationToken);

            var response = ApiResponse<LabOrderResponseDto>.Success(
                result,
                "Lab order created successfully.",
                HttpStatusCode.Created);

            return StatusCode(response.StatusCode, response);
        }

        
        [HttpGet("my")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorOrders(
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var result = await _labOrderService.GetDoctorOrdersAsync(
                doctorId,
                cancellationToken);

            var response = ApiResponse<List<LabOrderResponseDto>>.Success(
                result,
                "Doctor lab orders retrieved successfully.");

            return Ok(response);
        }

   
        [HttpGet("patient/my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientOrders(
            CancellationToken cancellationToken)
        {
            var patientId = GetUserId();

            var result = await _labOrderService.GetPatientOrdersAsync(
                patientId,
                cancellationToken);

            var response = ApiResponse<List<LabOrderResponseDto>>.Success(
                result,
                "Patient lab orders retrieved successfully.");

            return Ok(response);
        }

        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetLabOrderById(
            Guid id,
            CancellationToken cancellationToken)
        {
            
            var role = GetUserRole();
            var userId = role == "Lab"
                ? GetLabId()
                : GetUserId();

            var result = await _labOrderService.GetByIdAsync(
                id,
                userId,
                role,
                cancellationToken);

            var response = ApiResponse<LabOrderResponseDto>.Success(
                result,
                "Lab order retrieved successfully.");

            return Ok(response);
        }

        
        [HttpPost("{id:guid}/lab-request")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SelectLab(
            Guid id,
             SelectLabRequestDto request,
            CancellationToken cancellationToken)
        {
            var patientId = GetUserId();

            var result = await _labOrderService.SelectLabAsync(
                id,
                request,
                patientId,
                cancellationToken);

            var response = ApiResponse<LabOrderResponseDto>.Success(
                result,
                "Lab request submitted successfully.");

            return Ok(response);
        }

     
        [HttpGet("lab/my")]
        [Authorize(Roles = "Lab")]
        public async Task<IActionResult> GetLabOrders(
            CancellationToken cancellationToken)
        {
            var labId = GetLabId();

            var result = await _labOrderService.GetLabOrdersAsync(
                labId,
                cancellationToken);

            var response = ApiResponse<List<LabOrderResponseDto>>.Success(
                result,
                "Lab orders retrieved successfully.");

            return Ok(response);
        }

       
        [HttpPatch("{id:guid}/accept")]
        [Authorize(Roles = "Lab")]
        public async Task<IActionResult> AcceptOrder(
            Guid id,
            CancellationToken cancellationToken)
        {
            var labId = GetLabId();

            var result = await _labOrderService.AcceptOrderAsync(
                id,
                labId,
                cancellationToken);

            var response = ApiResponse<LabOrderResponseDto>.Success(
                result,
                "Lab order accepted successfully.");

            return Ok(response);
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Lab")]
        public async Task<IActionResult> RejectOrder(
            Guid id,
            CancellationToken cancellationToken)
        {
            var labId = GetLabId();

            var result = await _labOrderService.RejectOrderAsync(
                id,
                labId,
                cancellationToken);

            var response = ApiResponse<LabOrderResponseDto>.Success(
                result,
                "Lab order rejected successfully.");

            return Ok(response);
        }

     
        [HttpPatch("{id:guid}/sample-collected")]
        [Authorize(Roles = "Lab")]
        public async Task<IActionResult> MarkSampleCollected(
            Guid id,
            CancellationToken cancellationToken)
        {
            var labId = GetLabId();

            var result = await _labOrderService.MarkSampleCollectedAsync(
                id,
                labId,
                cancellationToken);

            var response = ApiResponse<LabOrderResponseDto>.Success(
                result,
                "Sample collection marked successfully.");

            return Ok(response);
        }

      
        [HttpPatch("{id:guid}/in-progress")]
        [Authorize(Roles = "Lab")]
        public async Task<IActionResult> MarkInProgress(
            Guid id,
            CancellationToken cancellationToken)
        {
            var labId = GetLabId();

            var result = await _labOrderService.MarkInProgressAsync(
                id,
                labId,
                cancellationToken);

            var response = ApiResponse<LabOrderResponseDto>.Success(
                result,
                "Lab testing started successfully.");

            return Ok(response);
        }

 
        [HttpPost("{id:guid}/results")]
        [Authorize(Roles = "Lab")]
        public async Task<IActionResult> SubmitResults(
            Guid id,
            SubmitLabResultsRequestDto request,
            CancellationToken cancellationToken)
        {
            var labId = GetLabId();

            var result = await _labOrderService.SubmitResultsAsync(
                id,
                request,
                labId,
                cancellationToken);

            var response = ApiResponse<List<LabResultResponseDto>>.Success(
                result,
                "Lab results submitted successfully.");

            return Ok(response);
        }

      
        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated or user ID is invalid.");
            }

            return parsedUserId;
        }
        private Guid GetLabId()
        {
            var labId = User.FindFirstValue("lab_id");

            if (!Guid.TryParse(labId, out var parsedLabId))
            {
                throw new UnauthorizedAccessException(
                    "Lab profile ID not found in JWT.");
            }

            return parsedLabId;
        }


        private string GetUserRole()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new UnauthorizedAccessException(
                    "User role not found.");
            }

            return role.ToLowerInvariant();
        }
    }
}
