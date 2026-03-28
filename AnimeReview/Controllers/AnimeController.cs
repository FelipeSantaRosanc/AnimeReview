using AnimeReview.DTOs.Anime;
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
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeService _animeService;

        public AnimeController(IAnimeService animeService)
        {
            _animeService = animeService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<AnimeResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] PaginationParams pagination,
            [FromQuery] AnimeFilterDto? filter)
        {
            var result = await _animeService.GetAllAsync(pagination, filter);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AnimeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _animeService.GetByIdAsync(id);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("Admin")]
        [ProducesResponseType(typeof(AnimeResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] AnimeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _animeService.CreateAsync(dto);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("Admin")]
        [ProducesResponseType(typeof(AnimeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] AnimeUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _animeService.UpdateAsync(id, dto);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _animeService.DeleteAsync(id);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return NoContent();
        }
    }
}