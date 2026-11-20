using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.LabOrders
{
    public class LabResultResponseDto
    {
        public Guid Id { get; set; }

        public Guid LabOrderItemId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string ResultValue { get; set; } = string.Empty;

        public string? Unit { get; set; }

        public string? ReferenceRange { get; set; }

        public string? Interpretation { get; set; }

        public string? Notes { get; set; }

        public string? PdfFileName { get; set; }
        public string? ContentType { get; set; }
        public long? FileSize { get; set; }
        public DateTimeOffset? UploadedAt { get; set; }

        public bool HasPdf { get; set; }

        public string? PdfDownloadUrl { get; set; }

        public DateTimeOffset ResultDate { get; set; }

        // Patient info for lab history
        public string? PatientName { get; set; }
        public string? Username { get; set; }
        public string? PatientEmail { get; set; }

        // Who submitted / uploaded this result
        public Guid? UploadedBy { get; set; }
        public string? UploadedByName { get; set; }

        // Lab that performed the test
        public string? LabName { get; set; }

        // Doctor who requested the test
        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorSpecialization { get; set; }
    }
}
