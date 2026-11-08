using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Admin
{
    public class LabDetailsResponseDto
    {
        public Guid LabId { get; set; }
        public Guid UserId { get; set; }

        public string LabName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }

        public string VerificationStatus { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}
