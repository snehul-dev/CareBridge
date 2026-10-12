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
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
          .Include(rt => rt.User)
              .ThenInclude(u => u.UserRoles)
                  .ThenInclude(ur => ur.Role)
          .Include(rt => rt.User)
              .ThenInclude(u => u.DoctorProfile)
          .Include(rt => rt.User)
              .ThenInclude(u => u.LabProfile)
          .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
