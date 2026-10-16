using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Domain.Entities
{
    public class DoctorProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SpecializationId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Qualification {  get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? HospitalOrClinicName { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Bio { get; set; } 
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;
        public Specialization Specialization { get; set; } = null!;

    }
}
