using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserDbContext _context;
        public AdminRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<DoctorProfile?> GetDoctorByIdAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            return await _context.DoctorProfiles
                  .Include(d => d.User)
                  .Include(d => d.Specialization)
                  .FirstOrDefaultAsync(d => d.Id == doctorId, cancellationToken);

        }

        public async Task<LabProfile?> GetLabByIdAsync(Guid labId, CancellationToken cancellationToken)
        {
            return await _context.LabProfiles
                .Include(l => l.User)
                .FirstOrDefaultAsync(
                    l => l.Id == labId,
                   cancellationToken);
        }

        public async Task<IEnumerable<DoctorProfile>> GetPendingDoctorsAsync(CancellationToken cancellationToken)
        {
           return await _context.DoctorProfiles
               .AsNoTracking()
              .Where(d => d.VerificationStatus == VerificationStatus.Pending)
              .Include(d => d.User)
              .Include(d => d.Specialization)
              .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LabProfile>> GetPendingLabsAsync(CancellationToken cancellationToken)
        {
            return await _context.LabProfiles
                .AsNoTracking()
                .Include(l => l.User)
                .Where(l => l.VerificationStatus == VerificationStatus.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId,
                cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {

            await _context.SaveChangesAsync(cancellationToken);
        }




    }
}
