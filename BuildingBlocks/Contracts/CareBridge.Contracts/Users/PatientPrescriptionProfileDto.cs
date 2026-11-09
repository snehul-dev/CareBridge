using System;
using System.Collections.Generic;
using System.Text;

namespace CareBridge.Contracts.Users
{
    public class PatientPrescriptionProfileDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
    }
}
