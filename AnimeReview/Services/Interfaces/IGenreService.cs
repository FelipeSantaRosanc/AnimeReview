using AnimeReview.Entities;

namespace AnimeReview.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAllGenresAsync();

    }
}
