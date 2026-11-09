using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Prescriptions
{
    public class PrescriptionPdfDto
    {
        public Guid PrescriptionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;

        public string? ClinicName { get; set; }
        public string DoctorAddress { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }

        public string? Notes { get; set; }

        public List<PrescriptionPdfItemDto> Items { get; set; } = new();
    }

    public class PrescriptionPdfItemDto
    {
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
