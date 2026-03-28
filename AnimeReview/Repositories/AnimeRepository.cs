using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AnimeReview.Data;
using AnimeReview.DTOs.Anime;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Shared;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Repositories
{
    public class AnimeRepository : IAnimeRepository
    {

        private readonly AnimeReviewDbContext _context;

        public AnimeRepository(AnimeReviewDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Anime>> GetAllAsync(PaginationParams pagination, AnimeFilterDto? filter = null)
        {
            var query = _context.Animes
                .Where(a => !a.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filter?.SearchTerm))
            {
                var search = filter.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Title.ToLower().Contains(search) ||
                    (a.Synopsis != null && a.Synopsis.ToLower().Contains(search)));
            }

            if (filter?.GenreId.HasValue == true)
            {
                query = query.Where(a => a.AnimeGenres.Any(ag => ag.GenreId == filter.GenreId.Value));
            }

            if (filter?.MinYear.HasValue == true)
            {
                query = query.Where(a => a.ReleaseYear >= filter.MinYear.Value);
            }

            if (filter?.MaxYear.HasValue == true)
            {
                query = query.Where(a => a.ReleaseYear <= filter.MaxYear.Value);
            }

            // Ordenação
            query = filter?.SortBy?.ToLower() switch
            {
                "year" => filter.SortDescending
                    ? query.OrderByDescending(a => a.ReleaseYear)
                    : query.OrderBy(a => a.ReleaseYear),
                "rating" => filter.SortDescending
                    ? query.OrderByDescending(a => a.Reviews.Average(r => (double?)r.Score) ?? 0)
                    : query.OrderBy(a => a.Reviews.Average(r => (double?)r.Score) ?? 0),
                _ => filter?.SortDescending == true
                    ? query.OrderByDescending(a => a.Title)
                    : query.OrderBy(a => a.Title)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
                .ToListAsync();

            return new PagedResult<Anime>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Anime?> GetByIdAsync(int id)
        {
            return await _context.Animes
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<Anime?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Animes
                .AsNoTracking()
                .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
                .Include(a => a.Reviews.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<bool> ExistsByTitleAsync(string title)
        {
            return await _context.Animes
                .AnyAsync(a => a.Title.ToLower() == title.ToLower() && !a.IsDeleted);
        }

        public async Task AddAsync(Anime anime)
        {
            await _context.Animes.AddAsync(anime);
        }

        public Task UpdateAsync(Anime anime)
        {
            anime.UpdatedAt = DateTime.UtcNow;
            _context.Animes.Update(anime);
            return Task.CompletedTask;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime != null)
            {
                anime.IsDeleted = true;
                anime.UpdatedAt = DateTime.UtcNow;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Animes
                .AnyAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task UpdateAverageRatingAsync(int animeId)
        {
            var anime = await _context.Animes
                .Include(a => a.Reviews.Where(r => !r.IsDeleted))
                .FirstOrDefaultAsync(a => a.Id == animeId);

            if (anime != null)
            {
                // Média será calculada no mapping, aqui apenas garantimos fresh data
                await _context.Entry(anime).ReloadAsync();
            }
        }

    }
}
