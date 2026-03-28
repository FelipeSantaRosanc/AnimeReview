using AnimeReview.DTOs.Auth;
using AnimeReview.Shared;

namespace AnimeReview.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> LoginAsync(string email, string password);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(string token, string refreshToken);
        Task<Result> LogoutAsync(string userId);

    }
}
