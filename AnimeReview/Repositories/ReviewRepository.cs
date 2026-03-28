using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Shared;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AnimeReviewDbContext _context;

        public ReviewRepository(AnimeReviewDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Review>> GetByAnimeIdAsync(int animeId, PaginationParams pagination)
        {
            var query = _context.Reviews
                .Where(r => r.AnimeId == animeId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Include(r => r.User)
                .ToListAsync();

            return new PagedResult<Review>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Anime)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<Review?> GetByUserAndAnimeAsync(string userId, int animeId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r =>
                    r.UserId == userId &&
                    r.AnimeId == animeId &&
                    !r.IsDeleted);
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public Task UpdateAsync(Review review)
        {
            review.UpdatedAt = DateTime.UtcNow;
            _context.Reviews.Update(review);
            return Task.CompletedTask;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                review.IsDeleted = true;
                review.UpdatedAt = DateTime.UtcNow;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reviews
                .AnyAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<bool> UserHasReviewedAsync(string userId, int animeId)
        {
            return await _context.Reviews
                .AnyAsync(r =>
                    r.UserId == userId &&
                    r.AnimeId == animeId &&
                    !r.IsDeleted);
        }
    }
}
