using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Infrastructure.Repositories
{
    public class DoctorAvailabilityRepository : IDoctorAvailabilityRepository
    {
        private readonly AppointmentDbContext _context;
        public DoctorAvailabilityRepository(AppointmentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DoctorAvailability availability, CancellationToken cancellationToken)
        {
            await _context.DoctorAvailabilities.AddAsync(
               availability,
               cancellationToken);
        }

        public void Delete(DoctorAvailability availability)
        {
            _context.DoctorAvailabilities.Remove(availability);
        }

        public async Task<IReadOnlyList<DoctorAvailability>> GetByDoctorAndDateRangeAsync(Guid doctorId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
        {
            return await _context.DoctorAvailabilities
                .AsNoTracking()
                .Where(x =>
                x.DoctorId == doctorId &&
                x.AvailableDate >= startDate &&
                x.AvailableDate <= endDate)
                .OrderBy(x => x.AvailableDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<DoctorAvailability>> GetByDoctorAndDayAsync(Guid doctorId,
            DateOnly date,
            CancellationToken cancellationToken)
        {
            return await _context.DoctorAvailabilities
                .AsNoTracking()
                .Where(x => x.DoctorId == doctorId &&
                x.AvailableDate == date &&
                x.IsAvailable)
                .OrderBy(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<DoctorAvailability?> GetByIdAsync(Guid id, Guid doctorId, 
            CancellationToken cancellationToken)
        {
            return await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(
                    x => x.Id == id && x.DoctorId == doctorId,
                    cancellationToken);
        }

        public async Task<bool> HasOverlappingAvailabilityAsync(Guid doctorId,
            DateOnly date, TimeOnly startTime, TimeOnly endTime, Guid? excludeId,
            CancellationToken cancellationToken)
        {
            var query = _context.DoctorAvailabilities
            .Where(x =>
            x.DoctorId == doctorId &&
            x.AvailableDate == date &&
            x.IsAvailable &&
            x.StartTime < endTime &&
            x.EndTime > startTime);

            // During update, exclude the current record
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

    

        public void Update(DoctorAvailability availability)
        {
            _context.DoctorAvailabilities.Update(availability);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
