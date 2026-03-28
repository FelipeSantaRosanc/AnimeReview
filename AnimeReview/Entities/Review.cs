using System.ComponentModel.DataAnnotations;

namespace AnimeReview.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 10)]
        public int Score { get; set; }  // ✅ Apenas um Score (removido Rating duplicado)

        [MaxLength(2000)]
        public string? Comment { get; set; }

        // Auditoria
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Relacionamentos
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int AnimeId { get; set; }
        public Anime Anime { get; set; } = null!;
    }
}
