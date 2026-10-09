using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
    public class AddressConfiguration:IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.AddressLine1)
                .IsRequired().HasMaxLength(150);
            builder.Property(x => x.AddressLine2)
               .HasMaxLength(150);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.PostalCode)
                .HasMaxLength(20);

            builder.Property(x => x.IsPrimary)
                .HasDefaultValue(false);
        }
    }
}
