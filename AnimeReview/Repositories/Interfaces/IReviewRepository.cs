using AnimeReview.Entities;

namespace AnimeReview.Repositories.Interfaces
{
        public interface IReviewRepository
        {
            Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId);

            Task AddAsync(Review review);

            Task DeleteAsync(int id);
        }
    
}
