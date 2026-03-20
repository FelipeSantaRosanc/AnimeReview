using AnimeReview.Data;
using AnimeReview.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync();
    }
}
