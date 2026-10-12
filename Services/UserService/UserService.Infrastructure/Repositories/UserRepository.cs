using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
           await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
              .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
              .Include(u => u.PatientProfile)
              .Include(u => u.DoctorProfile)
              .Include(u => u.LabProfile)
              .FirstOrDefaultAsync(u => u.Email == email);

        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .Include(u => u.PatientProfile)
                        .Include(u => u.DoctorProfile)
                        .Include(u => u.LabProfile)
                        .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine(
                        $"Entity: {entry.Metadata.ClrType.Name}");

                    Console.WriteLine(
                        $"State: {entry.State}");
                }

                throw;
            }
        }
    }
}
