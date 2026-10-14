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

        public async Task AddDoctorProfileAsync(DoctorProfile doctorProfile)
        {
            await _context.DoctorProfiles.AddAsync(doctorProfile);
        }

        public async Task AddPatientProfileAsync(PatientProfile patientProfile)
        {
            await _context.PatientProfiles.AddAsync(patientProfile);
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

        public async Task<User?> GetMyProfileAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

 
    }
}
