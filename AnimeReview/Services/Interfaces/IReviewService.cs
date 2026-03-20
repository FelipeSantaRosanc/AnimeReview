using AnimeReview.DTOs.Review;
using AnimeReview.Entities;

namespace AnimeReview.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(ReviewCreateDto dto);

        Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId);

        Task DeleteReviewAsync(int id);

    }
}
