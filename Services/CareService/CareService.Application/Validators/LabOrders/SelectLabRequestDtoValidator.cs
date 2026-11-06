using CareService.Application.DTOs.LabOrders;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Validators.LabOrders
{
    public class SelectLabRequestDtoValidator:AbstractValidator<SelectLabRequestDto>
    {
        public SelectLabRequestDtoValidator()
        {
            RuleFor(x => x.LabId)
                .NotEmpty()
                .WithMessage("Lab ID is required.");
        }
    }
}
