using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;

namespace AnimeReview.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly AnimeReviewDbContext _context;

        public UserRepository(AnimeReviewDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
