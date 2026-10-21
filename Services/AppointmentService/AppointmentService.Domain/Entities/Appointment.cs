using System;
using System.Collections.Generic;
using System.Text;
using AppointmentService.Domain.Enums;

namespace AppointmentService.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public AppointmentType AppointmentType { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public string? Reason { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
