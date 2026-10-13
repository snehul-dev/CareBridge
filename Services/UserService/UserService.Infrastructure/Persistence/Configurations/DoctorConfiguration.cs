using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
    public class DoctorConfiguration:IEntityTypeConfiguration<DoctorProfile>
    {
        public void Configure(EntityTypeBuilder<DoctorProfile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                .WithOne(x => x.DoctorProfile)
                .HasForeignKey<DoctorProfile>(x => x.UserId);

            builder.HasOne(x => x.Specialization)
                .WithMany(x => x.Doctors)
                .HasForeignKey(x => x.SpecializationId);

           

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.Property(x => x.LicenseNumber)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(x => x.LicenseNumber)
                .IsUnique();

            builder.Property(x => x.Qualification)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ExperienceYears)
                .IsRequired();

            builder.Property(x => x.ConsultationFee)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(x => x.City)
                .HasMaxLength(100);

            builder.Property(x => x.District)
                .HasMaxLength(100);

            builder.Property(x => x.Bio)
                .HasMaxLength(1000);

            builder.Property(x => x.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
            .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

        }
    }
}
