using System;
using System.Collections.Generic;
using System.Text;
using CareService.Application.DTOs.Consultations;
using FluentValidation;

namespace CareService.Application.Validators.Consultations
{
    public class ConsultationValidator : AbstractValidator<CreateConsultationRequestDto>
    {
        public ConsultationValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty()
                .WithMessage("Appointment ID is required.");

            RuleFor(x => x.ConsultationType)
                .IsInEnum()
                .WithMessage("Invalid consultation type.");
        }
    }

}
