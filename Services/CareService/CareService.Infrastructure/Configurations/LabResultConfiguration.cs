using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Configurations
{
    public class LabResultConfiguration:IEntityTypeConfiguration<LabResult>
    {
        public void Configure(EntityTypeBuilder<LabResult> builder)
        {
            builder.ToTable("LabResults");

            builder.HasKey(x => x.Id);

            
            builder.Property(x => x.LabOrderItemId)
                .IsRequired();

            builder.Property(x => x.LabOrderId)
                .IsRequired();

           
            builder.Property(x => x.ResultValue)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Unit)
                .HasMaxLength(50);

            builder.Property(x => x.ReferenceRange)
                .HasMaxLength(100);

            builder.Property(x => x.Interpretation)
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            
            builder.Property(x => x.ReportFileUrl)
                .HasMaxLength(1000);

            
            builder.Property(x => x.ResultDate)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

        
            builder.HasIndex(x => x.LabOrderItemId);

            builder.HasIndex(x => x.LabOrderId);


            builder.HasOne(x => x.LabOrderItem)
                .WithMany(x => x.LabResults)
                .HasForeignKey(x => x.LabOrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
