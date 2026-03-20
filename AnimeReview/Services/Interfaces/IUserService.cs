using AnimeReview.Entities;

namespace AnimeReview.Services.Interfaces
{
    public interface IUserService
    {

        Task<ApplicationUser?> GetUserAsync(string id);

        Task UpdateUserAsync(ApplicationUser user);
    }
}
