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

        
    }
}
