using System;
using System.Collections.Generic;
using System.Text;
using AppointmentService.Domain.Enums;

namespace AppointmentService.Domain.Entities
{
    public class ReferralRequest
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid FromDoctorId { get; set; }

        public Guid ToDoctorId { get; set; }

        public Guid AppointmentId { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public ReferralStatus Status { get; set; }  = ReferralStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Appointment Appointment { get; set; } = null!;

    }
}
