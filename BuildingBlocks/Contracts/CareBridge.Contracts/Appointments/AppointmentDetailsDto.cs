using System;
using System.Collections.Generic;
using System.Text;

namespace CareBridge.Contracts.Appointments
{
    public class AppointmentDetailsDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string AppointmentType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
