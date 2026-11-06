using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.LabOrders
{
    public class SubmitLabResultsRequestDto
    {
        public List<LabResultItemRequestDto> Results { get; set; } = new();
    }
    public class LabResultItemRequestDto
    {
        public Guid LabOrderItemId { get; set; }

        public string ResultValue { get; set; } = string.Empty;

        public string? Unit { get; set; }

        public string? ReferenceRange { get; set; }

        public string? Interpretation { get; set; }

        public string? Notes { get; set; }

        public string? ReportFileUrl { get; set; }
    }
}
