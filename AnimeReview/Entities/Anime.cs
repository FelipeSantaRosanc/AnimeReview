using System.ComponentModel.DataAnnotations;

namespace AnimeReview.Entities
{
    public class Anime
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Synopsis { get; set; }

        [MaxLength(500)]
        public string? CoverImage { get; set; }

        public int? ReleaseYear { get; set; }

        // Auditoria
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Relacionamentos
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<AnimeGenre> AnimeGenres { get; set; } = new List<AnimeGenre>();

    }
}
