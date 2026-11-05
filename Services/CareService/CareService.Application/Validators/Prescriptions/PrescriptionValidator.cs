using CareService.Application.DTOs.Prescriptions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Validators.Prescriptions
{
    public class PrescriptionValidator:AbstractValidator<CreatePrescriptionRequestDto>
    {
        public PrescriptionValidator()
        {
            RuleFor(x => x.ConsultationId)
                .NotEmpty()
                .WithMessage("Consultation ID is required.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters.");

            RuleFor(x => x.Items)
                .NotNull()
                .WithMessage("Prescription items are required.")
                .NotEmpty()
                .WithMessage("At least one prescription item is required.");

            RuleForEach(x => x.Items)
                .SetValidator(new PrescriptionItemValidator());
        }
    }
}
