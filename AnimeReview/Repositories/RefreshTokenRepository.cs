using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AnimeReviewDbContext _context;

        public RefreshTokenRepository(AnimeReviewDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<RefreshToken?> GetActiveByUserIdAsync(string userId)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt =>
                    rt.UserId == userId &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task RevokeAsync(string token, string? replacedByToken = null)
        {
            var refreshToken = await GetByTokenAsync(token);
            if (refreshToken != null && !refreshToken.IsRevoked)
            {
                refreshToken.IsRevoked = true;
                refreshToken.ReplacedByToken = replacedByToken;
            }
        }

        public async Task RevokeAllUserTokensAsync(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }
        }


    }
}
