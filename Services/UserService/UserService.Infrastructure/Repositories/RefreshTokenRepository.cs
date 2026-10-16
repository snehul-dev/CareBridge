using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UserDbContext _context;
        public RefreshTokenRepository(UserDbContext context)
        {
            _context = context;
        } 
        public async Task AddAsync(RefreshToken refreshToken , CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken , cancellationToken);
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash , CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
          .Include(rt => rt.User)
              .ThenInclude(u => u.UserRoles)
                  .ThenInclude(ur => ur.Role)
          .Include(rt => rt.User)
              .ThenInclude(u => u.DoctorProfile)
          .Include(rt => rt.User)
              .ThenInclude(u => u.LabProfile)
          .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash , cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
