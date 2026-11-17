using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Infrastructure.Repositories
{
    public class DoctorUnavailablePeriodRepository : IDoctorUnavailablePeriodRepository
    {
        private readonly AppointmentDbContext _context;

        public DoctorUnavailablePeriodRepository(AppointmentDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<DoctorUnavailablePeriod>> GetOverlappingAsync(Guid doctorId,
            DateOnly date, CancellationToken cancellationToken)
        {

            return await _context.DoctorUnavailablePeriods
                 .AsNoTracking()
                 .Where(x => x.DoctorId == doctorId &&
                 x.StartDate <= date && x.EndDate >= date
                 ).ToListAsync(cancellationToken);

        }

        public async Task<IReadOnlyList<DoctorUnavailablePeriod>> GetByDoctorAsync(Guid doctorId,
            CancellationToken cancellationToken)
        {
            return await _context.DoctorUnavailablePeriods
                .AsNoTracking()
                .Where(x => x.DoctorId == doctorId)
                .OrderBy(x => x.StartDate)
                .ThenBy(x => x.EndDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<DoctorUnavailablePeriod?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.DoctorUnavailablePeriods
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> HasOverlappingPeriodAsync(Guid doctorId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken)
        {
            return await _context.DoctorUnavailablePeriods
                .AsNoTracking()
                .AnyAsync(x => x.DoctorId == doctorId &&
                    x.StartDate <= endDate &&
                    x.EndDate >= startDate,
                    cancellationToken);
        }

        public async Task AddAsync(DoctorUnavailablePeriod period,
            CancellationToken cancellationToken)
        {
            await _context.DoctorUnavailablePeriods
                .AddAsync(period, cancellationToken);
        }

        public void Remove(DoctorUnavailablePeriod period)
        {
            _context.DoctorUnavailablePeriods.Remove(period);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
