using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using UserService.Application.Interfaces;


namespace UserService.Infrastructure.Security
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public (string Token, DateTime ExpiresAt) GenerateAccessToken(Guid userId, string email, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var secret = jwtSettings["Secret"]
                ?? throw new InvalidOperationException(
                    "JWT Secret is missing.");

            var issuer = jwtSettings["Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer is missing.");

            var audience = jwtSettings["Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience is missing.");

            if (!int.TryParse(
                    jwtSettings["ExpiryMinutes"],
                    out var expiryMinutes) ||
                expiryMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "JWT ExpiryMinutes must be a positive integer.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(secret);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT Secret must be at least 32 bytes long.");
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    role),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(keyBytes);

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler()
                .WriteToken(jwtToken);

            return (token, expiresAt);
        }
    }
}
