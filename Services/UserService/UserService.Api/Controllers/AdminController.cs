using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using UserService.Application.DTOs.Admin;
using UserService.Application.Interfaces;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    public class AdminController:ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("doctors/pending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PendingDoctorResponseDto>>>> GetPendingDoctors(
            CancellationToken cancellationToken)
        {
            var doctors = await _adminService.GetPendingDoctorsAsync(
                cancellationToken);

            var response = ApiResponse<IEnumerable<PendingDoctorResponseDto>>
                .Success(
                doctors,
                "Retrieved  pending doctors succesfully",
                HttpStatusCode.OK
                );


            return Ok(response);
        }

        [HttpGet("doctors/{id:guid}")]
        public async Task<ActionResult<ApiResponse<DoctorDetailsResponseDto>>> GetDoctorById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var doctor = await _adminService.GetDoctorByIdAsync(
                id, cancellationToken);


            var response = ApiResponse<DoctorDetailsResponseDto>
                .Success(
                doctor,
                "Doctor details retrieved successfully.",
                HttpStatusCode.OK
                );

            return Ok(response);
        
        }

        [HttpPatch("doctors/{id:guid}/approve")]
        public async Task<ActionResult<ApiResponse<string>>> ApproveDoctor(
            Guid id,
            CancellationToken cancellationToken)
        {
            var adminId = GetUserId();

            await _adminService.ApproveDoctorAsync(
                id, adminId, cancellationToken);

            var response = ApiResponse<string>.Success(
                "Doctor approved successfully.",
                "Doctor approval completed successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

        [HttpPatch("doctors/{id:guid}/reject")]
        public async Task<ActionResult<ApiResponse<string>>> RejectDoctor(
            Guid id,
            [FromBody] RejectReasonRequestDto request,
            CancellationToken cancellationToken)
        {
            var adminId = GetUserId();

            await _adminService.RejectDoctorAsync(
                id, adminId, request.Reason, cancellationToken);

            var response = ApiResponse<string>.Success(
                "Doctor rejected successfully.",
                "Doctor rejection completed successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }



 



        //Lab Controller

     
        [HttpGet("labs/pending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PendingLabResponseDto>>>>
            GetPendingLabs(CancellationToken cancellationToken)
        {
            var labs = await _adminService.GetPendingLabsAsync(
                cancellationToken);

            var response = ApiResponse<IEnumerable<PendingLabResponseDto>>.Success(
                labs,
                "Pending labs retrieved successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }


        [HttpGet("labs/{id:guid}")]
        public async Task<ActionResult<ApiResponse<LabDetailsResponseDto>>>
            GetLabById(Guid id, CancellationToken cancellationToken)
        {
            var lab = await _adminService.GetLabByIdAsync(
                id, cancellationToken);

            var response = ApiResponse<LabDetailsResponseDto>.Success(
                lab,
                "Lab details retrieved successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

    
        [HttpPatch("labs/{id:guid}/approve")]
        public async Task<ActionResult<ApiResponse<string>>> ApproveLab(
            Guid id,
            CancellationToken cancellationToken)
        {
            var adminId = GetUserId();

            await _adminService.ApproveLabAsync(
                id, adminId, cancellationToken);

            var response = ApiResponse<string>.Success(
                "Lab approved successfully.",
                "Lab approval completed successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

      
        [HttpPatch("labs/{id:guid}/reject")]
        public async Task<ActionResult<ApiResponse<string>>> RejectLab(
            Guid id,
            [FromBody] RejectReasonRequestDto request,
            CancellationToken cancellationToken)
        {
            var adminId = GetUserId();

            await _adminService.RejectLabAsync(
                id, adminId, request.Reason, cancellationToken);

            var response = ApiResponse<string>.Success(
                "Lab rejected successfully.",
                "Lab rejection completed successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }


        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AdminUserResponseDto>>>>
          GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _adminService.GetAllUsersAsync(
                cancellationToken);

            var response = ApiResponse<IEnumerable<AdminUserResponseDto>>.Success(
                users,
                "Users retrieved successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

        [HttpPatch("users/{id:guid}/activate")]
        public async Task<ActionResult<ApiResponse<string>>> ActivateUser(
                Guid id,
                CancellationToken cancellationToken)
        {
            var adminId = GetUserId();

            await _adminService.ActivateUserAsync(
                id,
                adminId,
                cancellationToken);

            var response = ApiResponse<string>.Success(
                "User activated successfully.",
                "Account activation completed successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

        [HttpPatch("users/{id:guid}/deactivate")]
        public async Task<ActionResult<ApiResponse<string>>> DeactivateUser(
                Guid id,
                 CancellationToken cancellationToken)
        {
            var adminId = GetUserId();

            await _adminService.DeactivateUserAsync(
                id,
                adminId,
                cancellationToken);

            var response = ApiResponse<string>.Success(
                "User deactivated successfully.",
                "Account deactivation completed successfully.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }



        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim not found.");

            if (!Guid.TryParse(userId, out var adminId))
                throw new UnauthorizedAccessException("Invalid user ID claim.");

            return adminId;
        }
    }
}
