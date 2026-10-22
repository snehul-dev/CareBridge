using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.DTOs
{
    public class DoctorSearchResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

        public Guid SpecializationId { get; set; }
        public string SpecializationName { get; set; } = string.Empty;

        public string Qualification { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }

        public string? HospitalOrClinicName { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Bio { get; set; }

        public bool IsVerified { get; set; }
    }
}
