using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;
using UserService.Domain.Enums;

namespace UserService.Application.Validators
{
    public class RegisterDoctorValidator:AbstractValidator<RegisterDoctorRequest>
    {
        public RegisterDoctorValidator()
        {
            RuleFor(x => x.FirstName)
                 .NotEmpty()
                 .WithMessage("First name is required.")
                 .MinimumLength(2)
                 .WithMessage("First name must be at least 2 characters.")
                 .MaximumLength(100)
                 .WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MinimumLength(2)
                .WithMessage("Last name must be at least 2 characters.")
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must contain at least 8 characters.")
                .MaximumLength(15)
                .WithMessage("Password cannot exceed 15 characters.")
                .Matches("[A-Za-z]")
                .WithMessage("Password must contain at least one letter.")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one digit.")
                .Matches(@"[^A-Za-z\d]")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{10,15}$")
                .WithMessage("Please enter a valid phone number.");

            RuleFor(x => x.SpecializationId)
                .NotEmpty()
                .WithMessage("Please select a specialization.");

            RuleFor(x => x.LicenseNumber)
                .NotEmpty()
                .WithMessage("Medical license number is required.")
                .MaximumLength(100)
                .WithMessage("Medical license number cannot exceed 100 characters.");

            RuleFor(x => x.Qualification)
                .NotEmpty()
                .WithMessage("Doctor qualification is required.")
                .MaximumLength(200)
                .WithMessage("Qualification cannot exceed 200 characters.");

            RuleFor(x => x.ExperienceYears)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Years of experience cannot be negative.");

            RuleFor(x => x.ConsultationFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Consultation fee cannot be negative.");

            RuleFor(x => x.HospitalOrClinicName)
                .MaximumLength(200)
                .WithMessage("Hospital or clinic name cannot exceed 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.District)
                .NotEmpty()
                .WithMessage("District is required.")
                .MaximumLength(100)
                .WithMessage("District cannot exceed 100 characters.");

            RuleFor(x => x.Bio)
                .MaximumLength(1000)
                .WithMessage("Doctor bio cannot exceed 1000 characters.");

            RuleFor(x => x.Gender)
                .Must(gender =>
                    string.IsNullOrWhiteSpace(gender) ||
                    (Enum.TryParse<Gender>(
                        gender, true, out var parsedGender) &&
                     Enum.IsDefined(parsedGender)))
                .WithMessage("Please select a valid gender.");

            RuleFor(x => x.DateOfBirth)
                .Must(date =>
                    !date.HasValue ||
                    date.Value.Date <= DateTime.UtcNow.Date)
                .WithMessage("Date of birth cannot be in the future.");

        }
    }
}
