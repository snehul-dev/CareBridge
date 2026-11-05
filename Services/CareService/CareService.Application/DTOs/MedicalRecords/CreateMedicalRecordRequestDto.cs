using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CareService.Application.DTOs.MedicalRecords
{
    public class CreateMedicalRecordRequestDto
    {
        public Guid ConsultationId { get; set; }

       
        public string? Symptoms { get; set; }

        public string Diagnosis { get; set; } = string.Empty;

        public string? ClinicalNotes { get; set; }

        
        public string? TreatmentPlan { get; set; }

        public DateOnly? FollowUpDate { get; set; }
    }
}
