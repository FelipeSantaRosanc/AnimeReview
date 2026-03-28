using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AnimeReview.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ProfileImage { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        // Auditoria
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Relacionamentos
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
