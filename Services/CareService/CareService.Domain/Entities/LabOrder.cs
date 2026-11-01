using CareService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Domain.Entities
{
    public class LabOrder
    {
        public Guid Id { get; set; }

        public Guid ConsultationId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? AssignedLabId { get; set; }
        public string? ClinicalNotes { get; set; }

        public LabOrderStatus LabOrderStatus { get; set; }

        public DateTimeOffset OrderedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? AssignedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public ICollection<LabOrderItem> Items { get; set; } = new List<LabOrderItem>();
        public Consultation Consultation { get; set; } = null!;

    }
}
