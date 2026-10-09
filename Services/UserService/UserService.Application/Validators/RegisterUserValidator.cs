using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;

namespace UserService.Application.Validators
{
    public class RegisterUserValidator:AbstractValidator<RegisterRequest>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Please enter the valid email");
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(15)
                .Matches("[A-Za-z]")
                .WithMessage("Password must contain at least one letter.")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one digit")
                .Matches(@"[^A-Za-z\d]")
                .WithMessage("Password must contain at least one special character.");

        }
    }
}
