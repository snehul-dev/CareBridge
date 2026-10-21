using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Infrastructure.Persistence;
using CareBridge.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;

using System.Text;

namespace AppointmentService.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppointmentDbContext _context;
        public AppointmentRepository(AppointmentDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            await _context.Appointments.AddAsync(appointment , cancellationToken);
        }

        public async Task<IReadOnlyList<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Where(x => x.DoctorId == doctorId &&
                       x.AppointmentDate == date)
                .OrderBy(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderBy(x => x.AppointmentDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }


        public async Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException
                {
                    SqlState: "23P01",
                    ConstraintName: "EX_Appointments_Doctor_Time_NoOverlap"
                })
            {
                throw new AppointmentSlotConflictException(
                    "This appointment slot has already been booked.");
            }
        }

    }
}
