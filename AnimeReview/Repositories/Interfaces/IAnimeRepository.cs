using AnimeReview.DTOs.Anime;
using AnimeReview.Entities;
using AnimeReview.Shared;

namespace AnimeReview.Repositories.Interfaces
{
    public interface IAnimeRepository
    {

        Task<PagedResult<Anime>> GetAllAsync(PaginationParams pagination, AnimeFilterDto? filter = null);
        Task<Anime?> GetByIdAsync(int id);
        Task<Anime?> GetByIdWithDetailsAsync(int id);
        Task<bool> ExistsByTitleAsync(string title);
        Task AddAsync(Anime anime);
        Task UpdateAsync(Anime anime);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task UpdateAverageRatingAsync(int animeId);

    }
}
