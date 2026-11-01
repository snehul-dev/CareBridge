using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Configurations
{
    public class MedicalRecordConfiguration:IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");

           
            builder.HasKey(x => x.Id);

         
            builder.Property(x => x.PatientId)
                .IsRequired();

            builder.Property(x => x.DoctorId)
                .IsRequired();

            builder.Property(x => x.ConsultationId)
                .IsRequired();

          
            builder.Property(x => x.Symptoms)
                .HasMaxLength(2000);

          
            builder.Property(x => x.Diagnosis)
                .IsRequired()
                .HasMaxLength(1000);

            
            builder.Property(x => x.ClinicalNotes)
                .HasMaxLength(5000);

            
            builder.Property(x => x.TreatmentPlan)
                .HasMaxLength(5000);

          
            builder.Property(x => x.FollowUpDate)
                .IsRequired(false);

         
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(x => x.PatientId);

            builder.HasIndex(x => x.DoctorId);

            builder.HasIndex(x => x.ConsultationId)
                .IsUnique();

        }
    }
}
