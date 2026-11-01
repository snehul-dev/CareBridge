using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Configurations
{
    public class PrescriptionItemConfiguration:IEntityTypeConfiguration<PrescriptionItem>
    {

        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");

            builder.HasKey(x => x.Id);

            
            builder.Property(x => x.PrescriptionId)
                .IsRequired();

            builder.Property(x => x.MedicineName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.GenericName)
                .HasMaxLength(200);

           
            builder.Property(x => x.DosageForm)
                .HasMaxLength(50);

          
            builder.Property(x => x.Dosage)
                .IsRequired()
                .HasMaxLength(100);

            
            builder.Property(x => x.Frequency)
                .IsRequired()
                .HasMaxLength(100);

            
            builder.Property(x => x.Duration)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Instructions)
                .HasMaxLength(1000);

            builder.Property(x => x.Quantity)
                .IsRequired(false);

          
            builder.Property(x => x.CreatedAt)
                .IsRequired();

         
            builder.HasIndex(x => x.PrescriptionId);

            builder.HasOne(x => x.Prescription)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
