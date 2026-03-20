using AnimeReview.Entities;

namespace AnimeReview.Repositories.Interfaces
{
    public interface IAnimeRepository
    {

        //listar todos os animes
        Task<IEnumerable<Anime>> GetAllAsync();

        //listar um anime por id
        Task<Anime?> GetByIdAsync(int id);

        // adicionar um anime
        Task AddAsync(Anime anime);

        // atualizar um anime
        Task UpdateAsync(Anime anime);

        // deletar um anime
        Task DeleteAsync(int id);

        Task<bool> SaveChangesAsync();

    }
}
