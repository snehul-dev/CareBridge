using System;

namespace AppointmentService.Application.DTOs
{
    public class UserSummaryDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
    }
}
