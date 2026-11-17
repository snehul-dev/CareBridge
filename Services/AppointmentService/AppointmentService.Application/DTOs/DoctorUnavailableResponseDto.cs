using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs
{
    public class DoctorUnavailableResponseDto
    {
        public Guid Id { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? Reason { get; set; }

        public int CancelledAppointmentsCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
