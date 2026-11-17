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
    }
}
