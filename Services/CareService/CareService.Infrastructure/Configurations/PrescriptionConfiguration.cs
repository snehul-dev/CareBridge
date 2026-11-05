using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Configurations
{
    public class PrescriptionConfiguration:IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

           
            builder.HasKey(p => p.Id);

           

            builder.Property(p => p.PatientId)
                .IsRequired();

            builder.Property(p => p.DoctorId)
                .IsRequired();

            builder.Property(p => p.ConsultationId)
                .IsRequired();

          
            builder.Property(p => p.Notes)
                .HasMaxLength(2000)
                .IsRequired(false);

      
            builder.Property(p => p.PdfFileUrl)
                .HasMaxLength(1000)
                .IsRequired(false);

            
            builder.Property(p => p.CreatedAt)
                .IsRequired();

          
            builder.HasIndex(p => p.PatientId);

            builder.HasIndex(p => p.DoctorId);

            builder.HasIndex(p => p.ConsultationId)
                .IsUnique();

            //one consultation one prescription

            builder.HasOne(p => p.Consultation)
                .WithOne(c => c.Prescription)
                .HasForeignKey<Prescription>(p => p.ConsultationId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
