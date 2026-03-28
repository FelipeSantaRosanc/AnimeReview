using AnimeReview.Entities;
using AnimeReview.Shared;

namespace AnimeReview.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<PagedResult<Review>> GetByAnimeIdAsync(int animeId, PaginationParams pagination);
        Task<Review?> GetByIdAsync(int id);
        Task<Review?> GetByUserAndAnimeAsync(string userId, int animeId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> UserHasReviewedAsync(string userId, int animeId);

    }

}