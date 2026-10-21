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
    }
}
