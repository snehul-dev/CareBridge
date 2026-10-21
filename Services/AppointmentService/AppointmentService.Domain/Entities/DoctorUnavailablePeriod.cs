using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Domain.Entities
{
    public class DoctorUnavailablePeriod
    {
        public Guid Id { get; set; }

        public Guid DoctorId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
