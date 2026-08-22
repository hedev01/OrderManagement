using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Application.Interfaces.Authentication;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Authentication
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var key = jwtSettings["Key"]
                      ?? throw new InvalidOperationException(
                          "JWT Key is not configured.");

            var issuer = jwtSettings["Issuer"]
                         ?? throw new InvalidOperationException(
                             "JWT Issuer is not configured.");

            var audience = jwtSettings["Audience"]
                           ?? throw new InvalidOperationException(
                               "JWT Audience is not configured.");

            var expirationMinutes =
                int.Parse(
                    jwtSettings["ExpirationMinutes"]
                    ?? "60");

            var claims = new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new(
                    ClaimTypes.Name,
                    user.Username),

                new(
                    ClaimTypes.Role,
                    user.Role.ToString())
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var expiresAt =
                DateTime.UtcNow.AddMinutes(
                    expirationMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
