using System;
using System.Collections.Generic;
using System.Text;
using CareService.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CareService.Infrastructure.Persistence
{
    public class CareDbContext:DbContext
    {
        public CareDbContext(DbContextOptions<CareDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CareDbContext).Assembly);
        }

        DbSet<Consultation> Consultations => Set<Consultation>();
        DbSet<LabOrder> LabOrders => Set<LabOrder>();
        DbSet<LabOrderItem> LabOrderItems => Set<LabOrderItem>();
        public DbSet<LabResult> LabResults => Set<LabResult>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    }
}
