using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UserDbContext _context;

        public RoleRepository(UserDbContext context)
        {
            _context = context;
        }
        public async Task<Role?> GetRoleByName(string roleName , CancellationToken cancellationToken)
        {
           return await _context.Roles.FirstOrDefaultAsync(n => n.Name == roleName , cancellationToken);

        }
    }
}
