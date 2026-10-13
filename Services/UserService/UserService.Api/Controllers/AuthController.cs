
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Auth;
using UserService.Application.Interfaces;
using UserService.Application.Validators;

namespace UserService.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly RegisterPatientValidator _patientvalidator;
        private readonly RegisterDoctorValidator _doctorvalidator;
        private readonly IAuthService _authService;
        private readonly RegisterLabValidator _labvalidator;

        public AuthController(RegisterPatientValidator patientvalidator,
            RegisterDoctorValidator doctorvalidator,
            RegisterLabValidator labvalidator,
            IAuthService authService
            )
        {
            _patientvalidator = patientvalidator;
            _doctorvalidator = doctorvalidator;
            _labvalidator = labvalidator;
            _authService = authService;

        }
        [HttpPost("Register/Patient")]
        public async Task<IActionResult> RegisterPatient(RegisterPatientRequest request)
        {
            var validationResult = await _patientvalidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(
                      error => new
                      {
                          Field = error.PropertyName,
                          Message = error.ErrorMessage
                      }
                    ));
            }

            await _authService.RegisterPatientAsync(request);
            return Created("", new
            {
                message = "Patient registered successfully."

            });


        }

        [HttpPost("Register/Doctor")]
        public async Task<IActionResult> RegisterDoctor(RegisterDoctorRequest request)
        {
            var validationResult = await _doctorvalidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(
                      error => new
                      {
                          Field = error.PropertyName,
                          Message = error.ErrorMessage
                      }
                    ));
            }

            await _authService.RegisterDoctorAsync(request);
            return Created("", new
            {
                message = "Doctor registration submitted successfully. Awaiting admin verification."
            });
        }

        [HttpPost("Register/Lab")]
        public async Task<IActionResult> RegisterLabAsync(RegisterLabRequest request)
        {
            var validationResult = await _labvalidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(
                      error => new
                      {
                          Field = error.PropertyName,
                          Message = error.ErrorMessage
                      }
                    ));
            }

            await _authService.RegisterLabAsync(request);

            return Created("", new
            {
                message = "Lab registration submitted successfully. Awaiting admin verification."

            });

        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
           var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

    }
}
