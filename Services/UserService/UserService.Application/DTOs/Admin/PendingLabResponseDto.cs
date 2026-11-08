using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Admin
{
    public class PendingLabResponseDto
    {
        public Guid LabId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string? City { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
