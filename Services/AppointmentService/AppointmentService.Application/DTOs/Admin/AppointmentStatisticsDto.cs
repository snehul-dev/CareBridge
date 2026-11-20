using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs.Admin
{
    public class AppointmentStatisticsDto
    {
        public int TotalAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

        public int RescheduledAppointments { get; set; }

        public int OngoingAppointments { get; set; }
    }
}
