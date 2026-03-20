using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Repositories
{
    public class ReviewRepository :   IReviewRepository
    {
        private readonly AnimeReviewDbContext _context;

        public ReviewRepository(AnimeReviewDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId)
        {
            return await _context.Reviews
                .Where(r => r.AnimeId == animeId)
                .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }


    }
}
