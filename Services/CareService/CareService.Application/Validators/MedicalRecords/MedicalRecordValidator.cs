using CareService.Application.DTOs.MedicalRecords;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CareService.Application.Validators.MedicalRecords
{
    public class MedicalRecordValidator:AbstractValidator<CreateMedicalRecordRequestDto>
    {
        public MedicalRecordValidator()
        {
            RuleFor(x => x.ConsultationId)
                .NotEmpty()
                .WithMessage("Consultation ID is required.");

          
            RuleFor(x => x.Symptoms)
                .MaximumLength(1000)
                .WithMessage("Symptoms cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Symptoms));

            
            RuleFor(x => x.Diagnosis)
                .NotEmpty()
                .WithMessage("Diagnosis is required.")
                .MaximumLength(2000)
                .WithMessage("Diagnosis cannot exceed 2000 characters.");

            
            RuleFor(x => x.ClinicalNotes)
                .MaximumLength(5000)
                .WithMessage("Clinical notes cannot exceed 5000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ClinicalNotes));

            
            RuleFor(x => x.TreatmentPlan)
                .MaximumLength(3000)
                .WithMessage("Treatment plan cannot exceed 3000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.TreatmentPlan));

        
            RuleFor(x => x.FollowUpDate)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Follow-up date cannot be in the past.")
                .When(x => x.FollowUpDate.HasValue);
        }
    }
}
