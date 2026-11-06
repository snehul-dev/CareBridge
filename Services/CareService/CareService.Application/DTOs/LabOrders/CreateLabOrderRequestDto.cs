using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.LabOrders
{
    public class CreateLabOrderRequestDto
    {
        public Guid ConsultationId { get; set; }

        public string? ClinicalNotes { get; set; }

        public List<LabOrderItemRequestDto> Items { get; set; } = new();
    }
}
