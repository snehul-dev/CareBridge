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

            RuleFor(x => x.Diagnosis)
                .NotEmpty()
                .WithMessage("Diagnosis is required.")
                .MaximumLength(2000)
                .WithMessage("Diagnosis cannot exceed 2000 characters.");

            RuleFor(x => x.TreatmentPlan)
                .MaximumLength(3000)
                .WithMessage("Treatment plan cannot exceed 3000 characters.");

            RuleFor(x => x.ClinicalNotes)
                .MaximumLength(5000)
                .WithMessage("Clinical notes cannot exceed 5000 characters.");

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
