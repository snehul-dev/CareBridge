using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Domain.Entities
{
    public class LabOrderItem
    {

        public Guid Id { get; set; }

        public Guid LabOrderId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string? TestCode { get; set; }
        public string? Instructions { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public LabOrder LabOrder { get; set; } = null!;

        public ICollection<LabResult> LabResults { get; set; } = null!;
    }
}
