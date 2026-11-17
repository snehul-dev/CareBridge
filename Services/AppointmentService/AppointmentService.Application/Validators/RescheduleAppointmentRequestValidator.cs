using AppointmentService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Validators
{
    public class RescheduleAppointmentRequestValidator:AbstractValidator<RescheduleAppointmentRequestDto>
    {
        public RescheduleAppointmentRequestValidator()
        {
            RuleFor(x => x.AppointmentDate)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Appointment date cannot be in the past.");

            RuleFor(x => x.StartTime)
                .Must(time => time >= TimeOnly.MinValue)
                .WithMessage("Invalid appointment start time.");
        }

    }
}
