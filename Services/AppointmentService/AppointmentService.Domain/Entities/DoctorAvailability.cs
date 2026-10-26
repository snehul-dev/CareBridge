using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Domain.Entities
{
    public class DoctorAvailability
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
