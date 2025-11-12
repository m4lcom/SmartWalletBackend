using Contracts.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartWallet.Application.Abstractions;
using SmartWallet.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartWallet.Infrastructure.ExternalServices;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
    {
        _config = configuration;
        _userRepository = userRepository;
    }

    public async Task<string?> AuthenticateAsync(LoginRequest request)
    {
        // --- buscar usuario por email ---
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null) return null;
        if (user.PasswordHash != request.Password) return null;

        return GenerateToken(user);
    }


    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
