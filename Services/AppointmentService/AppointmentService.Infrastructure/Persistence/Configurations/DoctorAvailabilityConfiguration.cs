using AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Infrastructure.Persistence.Configurations
{
    public class DoctorAvailabilityConfiguration:IEntityTypeConfiguration<DoctorAvailability>
    {
        public void Configure(EntityTypeBuilder<DoctorAvailability> builder)
        {
            builder.ToTable("DoctorAvailabilities");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoctorId)
                .IsRequired();

            builder.Property(x => x.DayOfWeek)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.StartTime)
                .HasColumnType("time without time zone")
                .IsRequired();

            builder.Property(x => x.EndTime)
                .HasColumnType("time without time zone")
                .IsRequired();

            builder.Property(x => x.IsAvailable)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.HasIndex(x => new
            {
                x.DoctorId,
                x.DayOfWeek
            });


            builder.ToTable(t => t.HasCheckConstraint(
                "CK_DoctorAvailabilities_TimeRange",
                "\"EndTime\" > \"StartTime\""));
        }
    }
}
