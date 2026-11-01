using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Configurations
{
    public class LabOrderConfiguration:IEntityTypeConfiguration<LabOrder>
    {
        public void Configure(EntityTypeBuilder<LabOrder> builder)
        {
            builder.ToTable("LabOrders");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.ConsultationId)
            .IsRequired();

            builder.Property(l => l.PatientId)
                .IsRequired();

            builder.Property(l => l.DoctorId)
                .IsRequired();

            builder.Property(l => l.AssignedLabId)
                .IsRequired(false);

            builder.Property(l => l.ClinicalNotes)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.Property(l => l.LabOrderStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(l => l.OrderedAt)
                .IsRequired();

            builder.Property(l => l.AssignedAt)
                .IsRequired(false);

            builder.Property(l => l.CompletedAt)
                .IsRequired(false);

            builder.Property(l => l.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(l => l.ConsultationId);

            builder.HasIndex(l => l.PatientId);

            builder.HasIndex(l => l.DoctorId);

            builder.HasIndex(l => l.AssignedLabId);

            builder.HasIndex(l => l.LabOrderStatus);
        }
    }
}
