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
    }
}
