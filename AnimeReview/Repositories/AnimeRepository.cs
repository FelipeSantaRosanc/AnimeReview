using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
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

        public async Task<IEnumerable<Anime>> GetAllAsync()
        {
            return await _context.Animes.ToListAsync();
        }

        public async Task<Anime?> GetByIdAsync(int id)
        {
            return await _context.Animes.FindAsync(id);
        }

        public async Task AddAsync(Anime anime)
        {
            await _context.Animes.AddAsync(anime);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Anime anime)
        {
            _context.Animes.Update(anime);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var anime = await _context.Animes.FindAsync(id);

            if (anime != null)
            {
                _context.Animes.Remove(anime);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
