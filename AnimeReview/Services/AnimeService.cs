using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;

namespace AnimeReview.Services
{
    public class AnimeService : IAnimeService
    {
        private readonly IAnimeRepository _animeRepository;

        public AnimeService(IAnimeRepository animeRepository)
        {
            _animeRepository = animeRepository;
        }

        public async Task<IEnumerable<Anime>> GetAllAsync()
        {
            return await _animeRepository.GetAllAsync();
        }

        public async Task<Anime?> GetByIdAsync(int id)
        {
            return await _animeRepository.GetByIdAsync(id);
        }

        public async Task<Anime> CreateAsync(Anime anime)
        {
            await _animeRepository.AddAsync(anime);
            return anime;
        }

        public async Task UpdateAsync(Anime anime)
        {
            await _animeRepository.UpdateAsync(anime);
        }

        public async Task DeleteAsync(int id)
        {
            await _animeRepository.DeleteAsync(id);
        }

    }
}
