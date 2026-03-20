using AnimeReview.Entities;

namespace AnimeReview.Repositories.Interfaces
{
    
        public interface IUserRepository
        {
            Task<ApplicationUser?> GetByIdAsync(string id);

            Task UpdateAsync(ApplicationUser user);
        }
    
}
