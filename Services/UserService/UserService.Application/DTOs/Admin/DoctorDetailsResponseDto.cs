using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Admin
{
    public class DoctorDetailsResponseDto
    {
        public Guid DoctorId { get; set; }
        public Guid UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Specialization { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; }
        public string? HospitalOrClinicName { get; set; }
        public string? Bio { get; set; }

        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }

        public string VerificationStatus { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}
