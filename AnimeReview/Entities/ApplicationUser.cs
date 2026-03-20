using Microsoft.AspNetCore.Identity;

namespace AnimeReview.Entities
{
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }

        public string? ProfileImage { get; set; }

        public string? Address { get; set; }

        public string? Bio { get; set; }

    }
}
