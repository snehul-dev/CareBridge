using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Configurations
{
    public class ConsultationConfiguration:IEntityTypeConfiguration<Consultation>
    {

        public void Configure(EntityTypeBuilder<Consultation> builder)
        {
            builder.ToTable("Consultations");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.AppointmentId)
                .IsRequired();

            builder.Property(c => c.DoctorId)
                .IsRequired();

            builder.Property(c => c.PatientId)
                .IsRequired();

            builder.Property(c => c.ConsultationType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(c => c.MeetingLink)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(c => c.StartedAt)
               .IsRequired(false);

            builder.Property(c => c.EndedAt)
                .IsRequired(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(c => c.AppointmentId)
                .IsUnique();

            builder.HasIndex(c => c.PatientId);
            builder.HasIndex(c => c.DoctorId);


            // One Consultation -> One MedicalRecord
            builder.HasOne(c => c.MedicalRecord)
                .WithOne(m => m.Consultation)
                .HasForeignKey<MedicalRecord>(m => m.ConsultationId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Consultation -> One Prescription
            builder.HasOne(c => c.Prescription)
                .WithOne(p => p.Consultation)
                .HasForeignKey<Prescription>(p => p.ConsultationId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Consultation -> Many LabOrders
            builder.HasMany(c => c.LabOrders)
                .WithOne(l => l.Consultation)
                .HasForeignKey(l => l.ConsultationId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
