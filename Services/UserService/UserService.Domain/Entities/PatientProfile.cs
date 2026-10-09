using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Domain.Entities
{
    public class PatientProfile
    {

        public Guid Id {  get; set; }
        public Guid UserId { get; set; }
        public string? BloodGroup { get; set; } = string.Empty;
        public string? EmergencyContactName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;


    }
}
