using CareService.Application.DTOs.Prescriptions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Validators.Prescriptions
{
    public class PrescriptionItemValidator:AbstractValidator<PrescriptionItemRequestDto>
    {
        public PrescriptionItemValidator()
        {
            RuleFor(x => x.MedicineName)
               .NotEmpty()
               .WithMessage("Medicine name is required.")
               .MaximumLength(200)
               .WithMessage("Medicine name cannot exceed 200 characters.");

            RuleFor(x => x.GenericName)
                .MaximumLength(200)
                .WithMessage("Generic name cannot exceed 200 characters.");

            RuleFor(x => x.DosageForm)
                .MaximumLength(50)
                .WithMessage("Dosage form cannot exceed 50 characters.");

            RuleFor(x => x.Dosage)
                .NotEmpty()
                .WithMessage("Dosage is required.")
                .MaximumLength(100)
                .WithMessage("Dosage cannot exceed 100 characters.");

            RuleFor(x => x.Frequency)
                .NotEmpty()
                .WithMessage("Frequency is required.")
                .MaximumLength(100)
                .WithMessage("Frequency cannot exceed 100 characters.");

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.")
                .MaximumLength(100)
                .WithMessage("Duration cannot exceed 100 characters.");

            RuleFor(x => x.Instructions)
                .MaximumLength(500)
                .WithMessage("Instructions cannot exceed 500 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .When(x => x.Quantity.HasValue)
                .WithMessage("Quantity must be greater than zero.");
        }
    }
}
