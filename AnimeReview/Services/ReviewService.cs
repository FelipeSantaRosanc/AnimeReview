using AnimeReview.DTOs.Review;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;

namespace AnimeReview.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review> CreateReviewAsync(ReviewCreateDto dto)
        {
            var review = new Review
            {
                AnimeId = dto.AnimeId,
                Score = dto.Score,
                Comment = dto.Comment
            };

            await _reviewRepository.AddAsync(review);

            return review;
        }

        public async Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId)
        {
            return await _reviewRepository.GetByAnimeIdAsync(animeId);
        }

        public async Task DeleteReviewAsync(int id)
        {
            await _reviewRepository.DeleteAsync(id);
        }



    }
}
