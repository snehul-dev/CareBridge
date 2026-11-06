using CareService.Application.DTOs.LabOrders;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Validators.LabOrders
{
    public class SubmitLabResultsRequestDtoValidator:AbstractValidator<SubmitLabResultsRequestDto>
    {
        public SubmitLabResultsRequestDtoValidator()
        {
            RuleFor(x => x.Results)
               .NotEmpty()
               .WithMessage("At least one lab result is required.");

            RuleForEach(x => x.Results)
                .SetValidator(new LabResultItemRequestDtoValidator());

            RuleFor(x => x.Results)
                .Must(results =>
                    results == null ||
                    results.Select(x => x.LabOrderItemId).Distinct().Count() == results.Count)
                .WithMessage("Duplicate lab order item IDs are not allowed.");
        }
    }
}
