using AnimeReview.Entities;

namespace AnimeReview.Services.Interfaces
{
    public interface IAnimeService
    {
        Task<IEnumerable<Anime>> GetAllAsync();

        Task<Anime?> GetByIdAsync(int id);

        Task<Anime> CreateAsync(Anime anime);

        Task UpdateAsync(Anime anime);

        Task DeleteAsync(int id);
    }
}
