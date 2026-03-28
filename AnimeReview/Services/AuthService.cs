using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AnimeReview.DTOs.Auth;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;
using AnimeReview.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AnimeReview.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        // Timing-safe: verificação dummy para evitar timing attacks
        if (user == null)
        {
            var dummyUser = new ApplicationUser { UserName = "dummy" };
            await _userManager.CheckPasswordAsync(dummyUser, "dummy");
            return Result<AuthResponseDto>.Unauthorized("Invalid credentials");
        }

        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            return Result<AuthResponseDto>.Unauthorized("Invalid credentials");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Revogar tokens antigos
            await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id);

            // Gerar tokens
            var jwtToken = GenerateJwtToken(user);
            var refreshTokenString = await GenerateAndSaveRefreshToken(user.Id);

            // Atualizar last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var duration = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60");

            var response = new AuthResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(duration),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Name = user.Name,
                    ProfileImage = user.ProfileImage
                }
            };

            return Result<AuthResponseDto>.Success(response);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Result<AuthResponseDto>.Unauthorized("Invalid token");

        var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

        if (storedRefreshToken == null ||
            storedRefreshToken.UserId != userId ||
            !storedRefreshToken.IsActive)
            return Result<AuthResponseDto>.Unauthorized("Invalid refresh token");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Revogar token antigo
            await _unitOfWork.RefreshTokens.RevokeAsync(refreshToken);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
                return Result<AuthResponseDto>.Unauthorized("User not found or inactive");

            // Gerar novos tokens
            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = await GenerateAndSaveRefreshToken(user.Id);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var duration = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60");

            var response = new AuthResponseDto
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(duration),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Name = user.Name,
                    ProfileImage = user.ProfileImage
                }
            };

            return Result<AuthResponseDto>.Success(response);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result> LogoutAsync(string userId)
    {
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Adicionar roles
        var roles = _userManager.GetRolesAsync(user).Result;
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var duration = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(duration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateAndSaveRefreshToken(string userId)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var token = Convert.ToBase64String(randomNumber);

        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        return token;
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}