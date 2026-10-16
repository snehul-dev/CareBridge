using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;

namespace UserService.Application.Validators
{
    public class RegisterLabValidator : AbstractValidator<RegisterLabRequest>
    {
        public RegisterLabValidator()
        {

            RuleFor(x => x.Email)
                 .NotEmpty()
                 .WithMessage("Email is required.")
                 .EmailAddress()
                 .WithMessage("Enter a valid email address.")
                 .MaximumLength(255)
                 .WithMessage("Email cannot exceed 255 characters.")
                 .Matches(@"^[a-zA-Z0-9]+([._%+-][a-zA-Z0-9]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+$")
                 .WithMessage("Please enter a valid email address.");

            // Password
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .MaximumLength(15)
                .WithMessage("Password cannot exceed 15 characters.")
                .Matches("[A-Za-z]")
                .WithMessage("Password must contain at least one letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");

            // Phone number

            RuleFor(x => x.PhoneNumber)
                 .NotEmpty()
                 .WithMessage("Phone number is required.")
                 .Matches(@"^\+?[0-9]{10,15}$")
                 .WithMessage("Please enter a valid phone number.");

            // Lab name
            RuleFor(x => x.LabName)
                .NotEmpty()
                .WithMessage("Lab name is required.")
                .MinimumLength(2)
                .WithMessage("Lab name must be at least 2 characters.")
                .MaximumLength(100)
                .WithMessage("Lab name cannot exceed 100 characters.");

            // License number
            RuleFor(x => x.LicenseNumber)
                .NotEmpty()
                .WithMessage("License number is required.")
                .MaximumLength(100)
                .WithMessage("License number cannot exceed 100 characters.");

            // Description (optional)
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.");

            // Address
            RuleFor(x => x.AddressLine)
                .NotEmpty()
                .WithMessage("Address is required.")
                .MaximumLength(250)
                .WithMessage("Address cannot exceed 250 characters.");

            // City
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters.");

            // District
            RuleFor(x => x.District)
                .NotEmpty()
                .WithMessage("District is required.")
                .MaximumLength(100)
                .WithMessage("District cannot exceed 100 characters.");

            // State
            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage("State is required.")
                .MaximumLength(100)
                .WithMessage("State cannot exceed 100 characters.");

            //Bio
            RuleFor(x => x.Bio)
                .MaximumLength(1000)
                .WithMessage("State cannot exceed 1000 characters.");
        }
    }
}
