using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
    public class PatientConfiguration:IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User)
                .WithOne(x => x.PatientProfile)
                .HasForeignKey<PatientProfile>(x => x.UserId);

            builder.HasIndex(x => x.UserId)
                .IsUnique();
            builder.Property(x => x.EmergencyContactName)
                .HasMaxLength(100);
            builder.Property(x => x.BloodGroup)
                .HasMaxLength(5);
        }

    }
}
