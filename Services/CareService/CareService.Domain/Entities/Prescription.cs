using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace CareService.Domain.Entities
{
    public class Prescription
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ConsultationId { get; set; }

        public string? Notes   { get; set; } = string.Empty;

        public string? PdfFileUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();

        public Consultation Consultation { get; set; } = null!;
    }
}
