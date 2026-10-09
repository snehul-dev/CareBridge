using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
     public class LabConfiguraion:IEntityTypeConfiguration<LabProfile>
    {
        public void Configure(EntityTypeBuilder<LabProfile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasIndex(x => x.LicenseNumber)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithOne(x => x.LabProfile)
                .HasForeignKey<LabProfile>(x => x.UserId);


            builder.Property(x => x.LabName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LicenseNumber)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Description)
                .HasMaxLength(1000);

        }
    }
}
