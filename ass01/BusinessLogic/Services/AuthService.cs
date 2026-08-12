using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ass01.BusinessLogic.DTOs.Auth;
using ass01.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ass01.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly AdminAccountConfigurationProvider _adminProvider;
    private readonly IAccountRepository _accountRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        AdminAccountConfigurationProvider adminProvider,
        IAccountRepository accountRepository,
        IConfiguration configuration)
    {
        _adminProvider = adminProvider;
        _accountRepository = accountRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Check Admin first from appsettings
        if (request.Email == _adminProvider.AdminEmail && request.Password == _adminProvider.AdminPassword)
        {
            return new LoginResponse
            {
                Email = request.Email,
                Role = "Admin",
                Token = GenerateJwtToken(request.Email, "Admin", 0)
            };
        }

        // Check SystemAccount
        var account = await _accountRepository.GetAccountByEmailAsync(request.Email);
        
        if (account == null || account.AccountPassword != request.Password)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        string role = account.AccountRole switch
        {
            1 => "Staff",
            2 => "Lecturer",
            _ => "Unknown"
        };

        if (role == "Unknown")
        {
            throw new UnauthorizedAccessException("Account has an invalid role.");
        }

        return new LoginResponse
        {
            Email = account.AccountEmail ?? string.Empty,
            Role = role,
            Token = GenerateJwtToken(account.AccountEmail ?? string.Empty, role, account.AccountId)
        };
    }

    private string GenerateJwtToken(string email, string role, short accountId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("AccountId", accountId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
