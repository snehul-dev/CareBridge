using AppointmentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs
{
    public class BookAppointmentRequestDto
    {
        public Guid DoctorId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public AppointmentType AppointmentType { get; set; }

        public string? Reason { get; set; }

        public string? Notes { get; set; }
    }
}
