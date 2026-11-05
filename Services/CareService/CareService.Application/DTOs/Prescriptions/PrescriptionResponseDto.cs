using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Prescriptions
{
    public class PrescriptionResponseDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid ConsultationId { get; set; }

        public string? Notes { get; set; }

        public string? PdfFileUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<PrescriptionItemResponseDto> Items { get; set; } = new();
    }
}
