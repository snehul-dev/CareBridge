using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Auth
{
    public class RegisterLabRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LabName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Bio { get; set; }
    }
}
