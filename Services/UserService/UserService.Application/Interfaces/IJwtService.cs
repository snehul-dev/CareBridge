using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Interfaces
{
    public interface IJwtService
    {

        (string Token, DateTime ExpiresAt) GenerateAccessToken(
           Guid userId,
           string email,
           string role);
    }
}
