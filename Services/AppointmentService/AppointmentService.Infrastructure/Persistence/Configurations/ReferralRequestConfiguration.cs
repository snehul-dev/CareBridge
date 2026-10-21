using AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Infrastructure.Persistence.Configurations
{
    public class ReferralRequestConfiguration:IEntityTypeConfiguration<ReferralRequest>
    {
        public void Configure(EntityTypeBuilder<ReferralRequest> builder)
        {
            builder.ToTable("ReferralRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PatientId).IsRequired();
            builder.Property(x => x.FromDoctorId).IsRequired();
            builder.Property(x => x.ToDoctorId).IsRequired();
            builder.Property(x => x.AppointmentId).IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.HasOne(x => x.Appointment)
                .WithMany()
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.AppointmentId);
            builder.HasIndex(x => new { x.ToDoctorId, x.Status });
        } 
    }
}
