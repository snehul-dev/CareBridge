using CareService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Consultations
{
    public class CreateConsultationRequestDto
    {
        public Guid AppointmentId { get; set; }
        public ConsultationType ConsultationType { get; set; }
        
    
    }
}
