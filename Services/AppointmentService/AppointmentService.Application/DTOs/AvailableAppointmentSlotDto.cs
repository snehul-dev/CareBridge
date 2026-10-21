using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs
{
    public class AvailableAppointmentSlotDto
    {
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
