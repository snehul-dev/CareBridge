using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Domain.Entities
{
    public class LabProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Description { get; set;  } 

        public User User { get; set; } = null!;

    }
}
