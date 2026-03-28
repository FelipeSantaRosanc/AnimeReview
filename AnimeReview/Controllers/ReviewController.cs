using System.Security.Claims;
using AnimeReview.DTOs.Review;
using AnimeReview.Services.Interfaces;
using AnimeReview.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AnimeReview.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableRateLimiting("Global")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in token");

        [HttpGet("anime/{animeId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<ReviewResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAnime(int animeId, [FromQuery] PaginationParams pagination)
        {
            var result = await _reviewService.GetByAnimeIdAsync(animeId, pagination);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _reviewService.GetByIdAsync(id);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize]
        [EnableRateLimiting("Authenticated")]
        [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _reviewService.CreateAsync(dto, userId);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [EnableRateLimiting("Authenticated")]
        [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _reviewService.UpdateAsync(id, dto, userId);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [EnableRateLimiting("Authenticated")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var result = await _reviewService.DeleteAsync(id, userId);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return NoContent();
        }
    }
}