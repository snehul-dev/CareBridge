using AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Infrastructure.Persistence
{
    public class AppointmentDbContext:DbContext
    {
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options) { }

        public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();
        public DbSet<ReferralRequest> ReferralRequests => Set<ReferralRequest>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<DoctorUnavailablePeriod> DoctorUnavailablePeriods => Set<DoctorUnavailablePeriod>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentDbContext).Assembly);
        }
    }
}
