using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.LabOrders
{
    public class LabOrderItemRequestDto
    {
        public string TestName { get; set; } = string.Empty;

        public string? TestCode { get; set; }
        public string? Instructions { get; set; }
    }
}
