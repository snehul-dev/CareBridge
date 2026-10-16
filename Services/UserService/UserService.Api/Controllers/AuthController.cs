
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserService.Application.DTOs.Auth;
using UserService.Application.Interfaces;
using UserService.Application.Validators;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterPatientValidator _patientValidator;
    private readonly RegisterDoctorValidator _doctorValidator;
    private readonly RegisterLabValidator _labValidator;
    private readonly IAuthService _authService;

    public AuthController(
        RegisterPatientValidator patientValidator,
        RegisterDoctorValidator doctorValidator,
        RegisterLabValidator labValidator,
        IAuthService authService)
    {
        _patientValidator = patientValidator;
        _doctorValidator = doctorValidator;
        _labValidator = labValidator;
        _authService = authService;
    }

    [HttpPost("Register/Patient")]
    public async Task<IActionResult> RegisterPatient(RegisterPatientRequest request , 
        CancellationToken cancellationToken)
    {
        var validationResult = await _patientValidator.ValidateAsync(request , cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error =>
                    $"{error.PropertyName}: {error.ErrorMessage}")
                .ToList();

            var failure = ApiResponse<object>.Fail(
                errors,
                "Validation failed",
                HttpStatusCode.BadRequest);

            return StatusCode(failure.StatusCode, failure);
        }

        await _authService.RegisterPatientAsync(request, cancellationToken);

        var response = ApiResponse<object>.Success(
            null,
            "Patient registered successfully.",
            HttpStatusCode.Created);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Register/Doctor")]
    public async Task<IActionResult> RegisterDoctor(
        RegisterDoctorRequest request , CancellationToken cancellationToken)
    {
        var validationResult = await _doctorValidator.ValidateAsync(request , cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error =>
                    $"{error.PropertyName}: {error.ErrorMessage}")
                .ToList();

            var failure = ApiResponse<object>.Fail(
                errors,
                "Validation failed",
                HttpStatusCode.BadRequest);

            return StatusCode(failure.StatusCode, failure);
        }

        await _authService.RegisterDoctorAsync(request , cancellationToken);

        var response = ApiResponse<object>.Success(
            null,
            "Doctor registration submitted successfully. Awaiting admin verification.",
            HttpStatusCode.Created);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Register/Lab")]
    public async Task<IActionResult> RegisterLabAsync(
        RegisterLabRequest request , CancellationToken cancellationToken)
    {
        var validationResult =
            await _labValidator.ValidateAsync(request ,cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error =>
                    $"{error.PropertyName}: {error.ErrorMessage}")
                .ToList();

            var failure = ApiResponse<object>.Fail(
                errors,
                "Validation failed",
                HttpStatusCode.BadRequest);

            return StatusCode(failure.StatusCode, failure);
        }

        await _authService.RegisterLabAsync(request,cancellationToken);

        var response = ApiResponse<object>.Success(
            null,
            "Lab registration submitted successfully. Awaiting admin verification.",
            HttpStatusCode.Created);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request , CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request,cancellationToken);

        var response = ApiResponse<object>.Success(
            result,
            "Login successful");

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request , CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(request , cancellationToken);

        var response = ApiResponse<object>.Success(
            result,
            "Token refreshed successfully");

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest request , CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request , cancellationToken);

        var response = ApiResponse<object>.Success(
            null,
            "Logged out successfully");

        return StatusCode(response.StatusCode, response);
    }
}
