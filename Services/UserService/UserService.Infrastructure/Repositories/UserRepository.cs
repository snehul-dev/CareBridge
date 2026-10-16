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

        public async Task AddAsync(User user , CancellationToken cancellationToken)
        {
           await _context.Users.AddAsync(user , cancellationToken);
        }

        public async Task AddDoctorProfileAsync(DoctorProfile doctorProfile , CancellationToken cancellationToken)
        {
            await _context.DoctorProfiles.AddAsync(doctorProfile , cancellationToken);
        }

        public async Task AddPatientProfileAsync(PatientProfile patientProfile , CancellationToken cancellationToken)
        {
            await _context.PatientProfiles.AddAsync(patientProfile,cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email , CancellationToken cancellationToken)
        {
            return await _context.Users
              .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
              .Include(u => u.PatientProfile)
              .Include(u => u.DoctorProfile)
              .Include(u => u.LabProfile)
              .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        }

        public async Task<User?> GetByIdAsync(Guid id , CancellationToken cancellationToken)
        {
            return await _context.Users
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .Include(u => u.PatientProfile)
                        .Include(u => u.DoctorProfile)
                        .Include(u => u.LabProfile)
                        .FirstOrDefaultAsync(u => u.Id == id , cancellationToken);
        }

        public async Task<User?> GetMyProfileAsync(Guid userId , CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId , cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

 
    }
}
