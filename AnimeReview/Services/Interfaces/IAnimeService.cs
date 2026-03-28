using AnimeReview.DTOs.Anime;
using AnimeReview.Entities;
using AnimeReview.Shared;

namespace AnimeReview.Services.Interfaces
{
    public interface IAnimeService
    {
        Task<Result<PagedResult<AnimeResponseDto>>> GetAllAsync(PaginationParams pagination, AnimeFilterDto? filter = null);
        Task<Result<AnimeResponseDto>> GetByIdAsync(int id);
        Task<Result<AnimeResponseDto>> CreateAsync(AnimeCreateDto dto);
        Task<Result<AnimeResponseDto>> UpdateAsync(int id, AnimeUpdateDto dto);
        Task<Result> DeleteAsync(int id);
    }
}
