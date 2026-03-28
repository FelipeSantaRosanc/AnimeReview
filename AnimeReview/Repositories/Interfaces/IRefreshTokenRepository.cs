using AnimeReview.Entities;

namespace AnimeReview.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetActiveByUserIdAsync(string userId);
        Task AddAsync(RefreshToken refreshToken);
        Task RevokeAsync(string token, string? replacedByToken = null);
        Task RevokeAllUserTokensAsync(string userId);

    }
}
