using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs.Admin
{
    public class PopularDoctorAppointmentDto
    {
        public Guid DoctorId { get; set; }

        public int AppointmentCount { get; set; }
    }
}
