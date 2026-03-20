using System.ComponentModel.DataAnnotations;

namespace AnimeReview.DTOs.Review
{
    public class ReviewCreateDto
    {
        [Required]
        public int AnimeId { get; set; }

        [Required]
        [Range(1,10, ErrorMessage = "Rating must be between 1 and 10.")]
        public int Score { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

    }
}
