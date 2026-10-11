using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByName(string roleName);
    }
}
