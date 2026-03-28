using AnimeReview.DTOs.Review;
using AnimeReview.Entities;
using AnimeReview.Shared;

namespace AnimeReview.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Result<PagedResult<ReviewResponseDto>>> GetByAnimeIdAsync(int animeId, PaginationParams pagination);
        Task<Result<ReviewResponseDto>> GetByIdAsync(int id);
        Task<Result<ReviewResponseDto>> CreateAsync(ReviewCreateDto dto, string userId);
        Task<Result<ReviewResponseDto>> UpdateAsync(int id, ReviewUpdateDto dto, string userId);
        Task<Result> DeleteAsync(int id, string userId);
        Task<Result> DeleteAsAdminAsync(int id);

    }
}
