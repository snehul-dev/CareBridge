using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    public class LabProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Description { get; set;  }
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
        public VerificationStatus VerificationStatus { get; set; }= VerificationStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public Guid? VerifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = null!;

    }
}
