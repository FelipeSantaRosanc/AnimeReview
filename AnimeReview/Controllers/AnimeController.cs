using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AnimeReview.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimeController : ControllerBase
    {

        private readonly IAnimeService animeService;

        public AnimeController(IAnimeService animeRepository)
        {
            animeService = animeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animes = await animeService.GetAllAsync();
            return Ok(animes);
        }

    }
}
