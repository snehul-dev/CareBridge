using CareBridge.Shared.Responses;
using CareService.Application.DTOs.MedicalRecords;
using CareService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CareService.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MedicalRecordsController:ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpPost]
        [Authorize(Roles ="Doctor")]
        public async Task<ActionResult<ApiResponse<MedicalRecordResponseDto>>> CreateRecords(CreateMedicalRecordRequestDto  request,
            CancellationToken cancellationToken)
        {
            var doctorId = GetUserId();

            var result = await  _medicalRecordService.CreateAsync(
                request,
                doctorId,
                cancellationToken
                );

            var response = ApiResponse<MedicalRecordResponseDto>
                .Success(result,
                "Medical record created successfully.",
                HttpStatusCode.Created);

            return CreatedAtAction(
                nameof(GetRecords),
                new { id = result.Id },
                response);
        }

        [HttpGet("Patient/{patientId:guid}")]
        [Authorize(Roles ="Patient,Doctor")]

        public async Task<ActionResult<ApiResponse<IEnumerable<MedicalRecordResponseDto>>>> GetPatientHistory(
            Guid patientId,
            CancellationToken cancellationToken
            )
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var result = await _medicalRecordService.GetPatientHistoryAsync(patientId,
                userId,
                role,
                cancellationToken);
            var response = ApiResponse<IEnumerable <MedicalRecordResponseDto>>
                .Success(
                result,
                "Patient history retrieved. ",
                HttpStatusCode.OK
                );
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles ="Patient,Doctor")]

        public async Task<ActionResult<ApiResponse<MedicalRecordResponseDto>>> GetRecords(Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var medicalrecords = await _medicalRecordService.GetByIdAsync(id,
                userId, role,
                cancellationToken);

            var response = ApiResponse<MedicalRecordResponseDto>
                .Success(medicalrecords,
                "Medical record retrieved.",
                HttpStatusCode.OK);

            return Ok(response);
        }



        private Guid GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!Guid.TryParse(userId ,out var id))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user ID in authentication token.");
            }

            return id;
        }

        private string GetUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role)
                ?? throw new UnauthorizedAccessException(
                    "User role not found in authentication token.");
        }
    }
}
