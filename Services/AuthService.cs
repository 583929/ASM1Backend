using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameApi.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Account> _hasher = new();

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>Đăng ký. Trả null nếu username đã tồn tại.</summary>
    public async Task<AccountResponse?> RegisterAsync(RegisterRequest request)
    {
        var exists = await _db.Accounts.AnyAsync(a => a.Username == request.Username);
        if (exists) return null;

        var account = new Account { Username = request.Username };
        account.PasswordHash = _hasher.HashPassword(account, request.Password);

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        return new AccountResponse { Id = account.Id, Username = account.Username };
    }

    /// <summary>Đăng nhập. Trả null nếu sai username HOẶC sai mật khẩu (cùng một kết quả).</summary>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Username == request.Username);
        if (account == null) return null;

        var result = _hasher.VerifyHashedPassword(account, account.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed) return null;

        return new LoginResponse
        {
            Token = CreateToken(account),
            Username = account.Username
        };
    }

    private string CreateToken(Account account)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
