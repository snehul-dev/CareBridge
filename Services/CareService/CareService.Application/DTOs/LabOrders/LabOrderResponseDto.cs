using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.LabOrders
{
    public  class LabOrderResponseDto
    {
        public Guid Id { get; set; }

        public Guid ConsultationId { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid? AssignedLabId { get; set; }

        public string? ClinicalNotes { get; set; }

        public string LabOrderStatus { get; set; } = string.Empty;

        public DateTimeOffset OrderedAt { get; set; }

        public DateTimeOffset? AssignedAt { get; set; }

        public DateTimeOffset? CompletedAt { get; set; }

        public List<LabOrderItemResponseDto> Items { get; set; } = new();
    }
}
