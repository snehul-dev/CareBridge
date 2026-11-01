using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Domain.Entities
{
    public class LabResult
    {
        public Guid Id { get; set; }

        public Guid LabOrderItemId { get; set; }
        public Guid LabOrderId { get; set; }

        public string ResultValue { get; set; } = string.Empty;
        public string? Unit { get; set; }

        public string? ReferenceRange { get; set; }
        public string? Interpretation { get; set; }
        public string? Notes { get; set; }

        // Reference to an uploaded report file
        public string? ReportFileUrl { get; set; }

        public DateTimeOffset ResultDate { get; set; }
            = DateTimeOffset.UtcNow;

        public DateTimeOffset CreatedAt { get; set; }
            = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }

        public LabOrderItem LabOrderItem { get; set; } = null!;
    }
}
