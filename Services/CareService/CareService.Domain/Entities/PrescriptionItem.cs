using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Domain.Entities
{
    public class PrescriptionItem
    {
        public Guid Id { get; set; }
        public Guid PrescriptionId { get; set; }

        public string MedicineName { get; set; } = string.Empty;
        public string? GenericName { get; set; }

        //tablet, capsule or syrup
        public string? DosageForm { get; set; }

        public string Dosage { get; set; } = string.Empty;

        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Instructions { get; set; }
        public int? Quantity { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Prescription Prescription { get; set; } = null!;


    }
}
