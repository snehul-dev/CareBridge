using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {

        private readonly UserDbContext _context;

        public DoctorRepository(UserDbContext context)
        {
            _context = context;
        }
        public async Task<DoctorProfile?> GetVerifiedByIdAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            return await _context.DoctorProfiles
         .AsNoTracking()
         .Where(d => d.Id == doctorId && d.IsVerified)
         .Include(d => d.User)
         .Include(d => d.Specialization)
         .FirstOrDefaultAsync(cancellationToken);

        }

        public async Task<IReadOnlyList<DoctorProfile>> SearchAsync(string? specialization, 
            string? city, string? district, CancellationToken cancellationToken)
        {
            var query = _context.DoctorProfiles
                .AsNoTracking()
                .Where(d => d.IsVerified)
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                var value = specialization.Trim();

                query = query.Where(d =>
                    d.Specialization.Name.ToLower() == value.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var value = city.Trim();

                query = query.Where(d =>
                    d.City != null &&
                    d.City.ToLower() == value.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(district))
            {
                var value = district.Trim();

                query = query.Where(d =>
                    d.District != null &&
                    d.District.ToLower() == value.ToLower());
            }

            return await query
                .OrderBy(d => d.User.FirstName)
                .ToListAsync(cancellationToken);
        }
    }
}
