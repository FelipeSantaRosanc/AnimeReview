using System.ComponentModel.DataAnnotations;

namespace AnimeReview.DTOs.Review
{
    public class ReviewUpdateDto
    {
        [Required(ErrorMessage = "Score is required")]
        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10")]
        public int Score { get; set; }

        [MaxLength(2000, ErrorMessage = "Comment must be less than 2000 characters")]
        public string? Comment { get; set; }

    }
}
