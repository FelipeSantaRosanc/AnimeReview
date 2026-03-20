using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly AnimeReviewDbContext _context;

        public GenreRepository(AnimeReviewDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }
    }

}
