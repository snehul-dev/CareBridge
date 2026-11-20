using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Admin
{
    public class PopularDoctorDto
    {
        public Guid DoctorId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public int AppointmentCount { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
