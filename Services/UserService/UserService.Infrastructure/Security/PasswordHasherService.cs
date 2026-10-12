using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Security
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
           return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
