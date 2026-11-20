using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs
{
    public class AppointmentResponseDto
    {
        public Guid Id { get; set; }

        public Guid DoctorId { get; set; }

        public Guid PatientId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? DoctorName { get; set; }

        public string? SpecializationName { get; set; }

        public string? PatientName { get; set; }

        public int? PatientAge { get; set; }

        public string AppointmentType { get; set; } = string.Empty;
    }
}
