using CareService.Application.DTOs.LabOrders;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Validators.LabOrders
{
    public class CreateLabOrderRequestDtoValidator : AbstractValidator<CreateLabOrderRequestDto>
    {
        public CreateLabOrderRequestDtoValidator()
        {
            RuleFor(x => x.ConsultationId)
                .NotEmpty()
                .WithMessage("Consultation ID is required.");

            RuleFor(x => x.ClinicalNotes)
                .MaximumLength(2000)
                .WithMessage("Clinical notes cannot exceed 2000 characters.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one lab test is required.");

            RuleForEach(x => x.Items)
                .SetValidator(new LabOrderItemRequestDtoValidator());
        }
    }
}
