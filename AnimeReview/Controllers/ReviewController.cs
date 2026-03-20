using AnimeReview.DTOs.Review;
using AnimeReview.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeReview.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(ReviewCreateDto dto)
        {
            var review = await _reviewService.CreateReviewAsync(dto);

            return Ok(review);
        }
    }
}