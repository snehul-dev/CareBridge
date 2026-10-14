using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Enums;

namespace UserService.Application.DTOs.Users
{
    public class MyProfileResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }

        public PatientProfileResponse? PatientProfile { get; set; }
        public DoctorProfileResponse? DoctorProfile { get; set; }
        public LabProfileResponse? LabProfile { get; set; }
    }
    public class PatientProfileResponse
    {
        public string? BloodGroup { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? MedicalHistory { get; set; }
    }
    public class DoctorProfileResponse
    {
        public Guid SpecializationId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? HospitalOrClinicName { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
    }

    public class LabProfileResponse
    {
        public string LabName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
    }
}
