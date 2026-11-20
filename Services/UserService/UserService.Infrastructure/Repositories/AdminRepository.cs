using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Admin;
using UserService.Application.Interfaces;
using UserService.Application.Services;
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
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
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

        public async Task<RoleCountsDto> GetRoleCountsAsync(
            CancellationToken cancellationToken)
        {
            var groups = await _context.UserRoles
                .AsNoTracking()
                .GroupBy(ur => ur.Role.Name)
                .Select(g => new
                {
                    Role = g.Key,
                    Count = g.Select(x => x.UserId).Distinct().Count()
                })
                .ToListAsync(cancellationToken);

            var counts = groups.ToDictionary(
                x => x.Role,
                x => x.Count,
                StringComparer.OrdinalIgnoreCase);

            return new RoleCountsDto
            {
                TotalPatients = counts.GetValueOrDefault("Patient", 0),
                TotalDoctors = counts.GetValueOrDefault("Doctor", 0),
                TotalLabs = counts.GetValueOrDefault("Lab", 0)
            };
        }

        public async Task<IReadOnlyList<DoctorSummaryDto>> GetDoctorSummariesAsync(
            IReadOnlyCollection<Guid> userIds,
            CancellationToken cancellationToken)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return Array.Empty<DoctorSummaryDto>();
            }

            var users = await _context.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.UpdatedAt,
                    SpecializationName =
                        u.DoctorProfile != null
                            ? u.DoctorProfile.Specialization.Name
                            : null
                })
                .ToListAsync(cancellationToken);

            return users
                .Select(u => new DoctorSummaryDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfileImageUrl = ProfilePictureUrls.Build(
                        u.Id,
                        u.UpdatedAt),
                    SpecializationName = u.SpecializationName
                })
                .ToList();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {

            await _context.SaveChangesAsync(cancellationToken);
        }




    }
}
