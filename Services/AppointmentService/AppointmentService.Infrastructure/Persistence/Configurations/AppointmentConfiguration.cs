using System;
using System.Collections.Generic;
using System.Text;
using AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentService.Infrastructure.Persistence.Configurations
{
    public class AppointmentConfiguration:IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PatientId)
                .IsRequired();

            builder.Property(x => x.DoctorId)
                .IsRequired();

            builder.Property(x => x.AppointmentDate)
            .HasColumnType("date")
            .IsRequired();

            builder.Property(x => x.StartTime)
                .HasColumnType("time without time zone")
                .IsRequired();

            builder.Property(x => x.EndTime)
                .HasColumnType("time without time zone")
                .IsRequired();

            builder.Property(x => x.AppointmentType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.HasIndex(x => new
            {
                x.DoctorId,
                x.AppointmentDate,
                x.Status
            });

            builder.HasIndex(x => new
            {
                x.PatientId,
                x.AppointmentDate
            });

            builder.ToTable(t => t.HasCheckConstraint(
            "CK_Appointments_TimeRange",
            "\"EndTime\" > \"StartTime\""));


        }
    }
}
