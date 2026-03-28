using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly AnimeReviewDbContext _context;

    public GenreRepository(AnimeReviewDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _context.Genres
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<List<Genre>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Genres
            .Where(g => ids.Contains(g.Id))
            .ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _context.Genres.FindAsync(id);
    }
}