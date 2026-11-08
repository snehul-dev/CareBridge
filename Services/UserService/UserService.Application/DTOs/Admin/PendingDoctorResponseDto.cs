using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Admin
{
    public class PendingDoctorResponseDto
    {
        public Guid DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
