using CareService.Application.DTOs.LabOrders;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Validators.LabOrders
{
    public class LabResultItemRequestDtoValidator:AbstractValidator<LabResultItemRequestDto>
    {

        public LabResultItemRequestDtoValidator()
        {
            RuleFor(x => x.LabOrderItemId)
               .NotEmpty()
               .WithMessage("Lab order item ID is required.");

            RuleFor(x => x.ResultValue)
                .NotEmpty()
                .WithMessage("Result value is required.")
                .MaximumLength(500)
                .WithMessage("Result value cannot exceed 500 characters.");

            RuleFor(x => x.Unit)
                .MaximumLength(50);

            RuleFor(x => x.ReferenceRange)
                .MaximumLength(100);

            RuleFor(x => x.Interpretation)
                .MaximumLength(500);

            RuleFor(x => x.Notes)
                .MaximumLength(2000);

            RuleFor(x => x.ReportFileUrl)
                .MaximumLength(1000);
        }
    }
}
