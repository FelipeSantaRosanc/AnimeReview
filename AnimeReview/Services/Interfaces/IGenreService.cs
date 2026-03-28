using AnimeReview.DTOs.Anime;
using AnimeReview.Entities;
using AnimeReview.Shared;

namespace AnimeReview.Services.Interfaces
{
    public interface IGenreService
    {
        Task<Result<IEnumerable<GenreDto>>> GetAllAsync();
        Task<Result<GenreDto>> GetByIdAsync(int id);
        Task<Result<GenreDto>> CreateAsync(string name);
        Task<Result> DeleteAsync(int id);

    }
}
