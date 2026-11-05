using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Prescriptions
{
    public class PrescriptionItemResponseDto
    {
        public Guid Id { get; set; }

        public string MedicineName { get; set; } = string.Empty;

        public string? GenericName { get; set; }

        public string? DosageForm { get; set; }

        public string Dosage { get; set; } = string.Empty;

        public string Frequency { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;

        public string? Instructions { get; set; }

        public int? Quantity { get; set; }
    }
}
