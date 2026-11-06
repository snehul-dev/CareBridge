using CareService.Application.DTOs.LabOrders;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Validators.LabOrders
{
    public class LabOrderItemRequestDtoValidator : AbstractValidator<LabOrderItemRequestDto>
    {
        public LabOrderItemRequestDtoValidator()
        {
            RuleFor(x => x.TestName)
                .NotEmpty()
                .WithMessage("Test name is required.")
                .MaximumLength(200)
                .WithMessage("Test name cannot exceed 200 characters.");

            RuleFor(x => x.TestCode)
                .MaximumLength(50)
                .WithMessage("Test code cannot exceed 50 characters.");

            RuleFor(x => x.Instructions)
                .MaximumLength(1000)
                .WithMessage("Instructions cannot exceed 1000 characters.");
        }
    }
   
}
