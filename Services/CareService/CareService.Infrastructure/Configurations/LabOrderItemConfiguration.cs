using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Configurations
{
    public class LabOrderItemConfiguration:IEntityTypeConfiguration<LabOrderItem>
    {
        public void Configure(EntityTypeBuilder<LabOrderItem> builder)
        {
            builder.ToTable("LabOrderItems");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.LabOrderId)
            .IsRequired();

            builder.Property(l => l.TestName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.TestCode)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(l => l.Instructions)
                .HasMaxLength(1000)
                .IsRequired(false);

            
            builder.Property(l => l.CreatedAt)
                .IsRequired();

           
            builder.HasIndex(l => l.LabOrderId);

            builder.HasOne(l => l.LabOrder)
                .WithMany(l => l.Items)
                .HasForeignKey(l => l.LabOrderId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
