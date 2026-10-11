using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;

namespace UserService.Application.Validators
{
    public class RegisterPatientValidator:AbstractValidator<RegisterPatientRequest>
    {
        public RegisterPatientValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must contain at least 6 characters.")
                .MaximumLength(15)
                .WithMessage("Password cannot exceed 15 characters.")
                .Matches("[A-Za-z]")
                .WithMessage("Password must contain at least one letter.")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one digit.")
                .Matches(@"[^A-Za-z\d]")
                .WithMessage("Password must contain at least one special character.");

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

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{10,15}$")
                .WithMessage("Please enter a valid phone number.");

            RuleFor(x => x.Gender)
                .Must(gender =>
                    string.IsNullOrWhiteSpace(gender) ||
                    (Enum.TryParse<UserService.Domain.Enums.Gender>(
                        gender, true, out var parsedGender) &&
                     Enum.IsDefined(parsedGender)))
                .WithMessage("Please enter a valid gender.");

            RuleFor(x => x.DateOfBirth)
                .Must(date =>
                    !date.HasValue ||
                    date.Value.Date <= DateTime.UtcNow.Date)
                .WithMessage("Date of birth cannot be in the future.");

        }
    }
}
