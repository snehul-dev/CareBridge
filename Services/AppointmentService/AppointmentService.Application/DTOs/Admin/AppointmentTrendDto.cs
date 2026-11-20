using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs.Admin
{
    public class AppointmentTrendDto
    {
        public string Month { get; set; } = string.Empty;

        public int Completed { get; set; }

        public int Ongoing { get; set; }

        public int Cancelled { get; set; }
    }
}
