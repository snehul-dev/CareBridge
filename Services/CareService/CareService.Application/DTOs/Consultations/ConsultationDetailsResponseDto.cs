using CareService.Domain.Entities;
using CareService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Consultations
{
    public class ConsultationDetailsResponseDto
    {
        public Guid Id { get; set; }

        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }

        public ConsultationType ConsultationType { get; set; }
        public ConsultationStatus Status { get; set; }

        public string? MeetingLink { get; set; }

        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
