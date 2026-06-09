using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.SessionDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DarkKitchen.Services;

public class SessionService(
    IUserRepository userRepository,
    IRolePermissionsService rolePermissionsService,
    IConfiguration configuration) : ISessionService
{
    public LoginResponseDto Login(LoginDto loginDto)
    {
        var user = userRepository.GetByEmail(loginDto.email!);
        if(user == null || user.IsDeleted || !BCrypt.Net.BCrypt.Verify(loginDto.password, user.Password))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        return new LoginResponseDto(GenerateToken(user));
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var permissions = rolePermissionsService.GetPermissions(user.Role);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("name", user.Name)
        };
        claims.AddRange(permissions.Select(p => new Claim("permission", p.ToString())));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
