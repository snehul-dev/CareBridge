using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Enums;

namespace UserService.Application.DTOs.Users
{
    public class UpdateMyProfileRequest
    {
        //common user 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    

        //patient profile 
        public string? BloodGroup { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? MedicalHistory { get; set; }


        //Doctor Profile
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? HospitalOrClinicName { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? DoctorBio { get; set; }

        //Lab Profile
        public string? LabName { get; set; }
        public string? Description { get; set; }
        public string? AddressLine { get; set; }
        public string? LabCity { get; set; }
        public string? LabDistrict { get; set; }
        public string? State { get; set; }
        public string? LabBio { get; set; }
    }
}
