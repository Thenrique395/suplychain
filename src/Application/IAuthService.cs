using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using SupplyChain.Domain;

namespace SupplyChain.Application;

public interface IAuthService
{
    string HashPassword(string password);
    bool Verify(string password, string hash);
    string GenerateToken(Employee user, IConfiguration config);
}

public class AuthService : IAuthService
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);

    public string GenerateToken(Employee user, IConfiguration config)
    {
        var key = Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Key") ?? "VerySecretKey_DoNotUseInProd_ChangeMe");
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("role", user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}