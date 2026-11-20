using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Admin
{
    public class DoctorAppointmentCountDto
    {
        public Guid DoctorId { get; set; }

        public int AppointmentCount { get; set; }
    }
}
