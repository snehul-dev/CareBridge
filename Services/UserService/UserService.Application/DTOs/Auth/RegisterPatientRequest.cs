using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Auth
{
    public class RegisterPatientRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Gender { get; set; } 
        public DateTime? DateOfBirth { get; set; }
    }
}
