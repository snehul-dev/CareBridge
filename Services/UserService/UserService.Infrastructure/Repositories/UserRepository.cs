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
            var entries = _context.ChangeTracker
                .Entries<DoctorProfile>()
                .ToList();

            foreach (var entry in entries)
            {
                Console.WriteLine(
                    $"DoctorProfile State: {entry.State}");

                Console.WriteLine(
                    $"DoctorProfile Id: {entry.Entity.Id}");

                Console.WriteLine(
                    $"DoctorProfile UserId: {entry.Entity.UserId}");
            }

            await _context.SaveChangesAsync();
        }
    }
}
