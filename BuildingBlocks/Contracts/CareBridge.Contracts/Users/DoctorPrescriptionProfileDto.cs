using System;
using System.Collections.Generic;
using System.Text;

namespace CareBridge.Contracts.Users
{
    public class DoctorPrescriptionProfileDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Qualification { get; set; }
        public string? HospitalOrClinicName { get; set; }
        public string? DoctorAddress { get; set; }
        public string? City { get; set; }
        public string? SpecializationName { get; set; }
    }
}
