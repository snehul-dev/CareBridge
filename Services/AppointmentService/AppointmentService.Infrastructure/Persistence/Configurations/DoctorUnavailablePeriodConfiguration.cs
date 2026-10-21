using AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Infrastructure.Persistence.Configurations
{
    public class DoctorUnavailablePeriodConfiguration:IEntityTypeConfiguration<DoctorUnavailablePeriod>
    {
        public void Configure(EntityTypeBuilder<DoctorUnavailablePeriod> builder)
        {
            builder.ToTable("DoctorUnavailablePeriods");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoctorId).IsRequired();

            builder.Property(x => x.StartDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.EndDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_DoctorUnavailablePeriods_DateRange",
                "\"EndDate\" >= \"StartDate\""));

            builder.HasIndex(x => new { x.DoctorId, x.StartDate, x.EndDate });
        }
    }
}
