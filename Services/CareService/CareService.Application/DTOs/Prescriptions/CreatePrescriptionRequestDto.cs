using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Prescriptions
{
    public class CreatePrescriptionRequestDto
    {

        public Guid ConsultationId { get; set; }
        public string? Notes { get; set; } = string.Empty;
        public List<PrescriptionItemRequestDto> Items { get; set; } = new();
    }
}
