using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs
{
    public class RescheduleAppointmentRequestDto
    {
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
