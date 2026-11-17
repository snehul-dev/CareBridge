using AppointmentService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Validators
{
    public class BookAppointmentRequestValidator:AbstractValidator<BookAppointmentRequestDto>
    {
        public BookAppointmentRequestValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty()
                .WithMessage("Doctor ID is required.");

            RuleFor(x => x.AppointmentDate)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Appointment date cannot be in the past.");

            RuleFor(x => x.StartTime)
                .Must(time => time >= TimeOnly.MinValue)
                .WithMessage("Invalid appointment start time.");

            RuleFor(x => x.AppointmentType)
                .IsInEnum()
                .WithMessage("Invalid appointment type.");

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }
    }
}
