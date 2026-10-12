using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Auth
{
    public class LoginRequest
    {
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
