using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class LabRepository : ILabRepository
    {
        private readonly UserDbContext _context;

        public LabRepository(UserDbContext context)
        {
            _context = context;
        }
        public async Task<LabProfile?> GetVerifiedLabByIdAsync(Guid labId, CancellationToken cancellationToken)
        {
            return await _context.LabProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                x => x.Id == labId && x.IsVerified,
                cancellationToken);
        }

        public async Task<List<LabProfile>> GetVerifiedLabsAsync(CancellationToken cancellationToken)
        {
            return await _context.LabProfiles
              .AsNoTracking()
              .Where(x => x.IsVerified)
              .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsActiveAndVerifiedAsync(Guid labId, CancellationToken cancellationToken)
        {
            return await _context.LabProfiles
               .AsNoTracking()
               .AnyAsync(
                   x => x.Id == labId &&
                        x.IsVerified &&
                        x.IsActive,
                   cancellationToken);
        }
    }
}
