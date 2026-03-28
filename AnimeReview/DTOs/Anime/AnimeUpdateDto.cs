using System.ComponentModel.DataAnnotations;

namespace AnimeReview.DTOs.Anime
{
    public class AnimeUpdateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title must be less than 200 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000, ErrorMessage = "Synopsis must be less than 2000 characters")]
        public string? Synopsis { get; set; }

        [Range(1900, 2100, ErrorMessage = "Release year must be between 1900 and 2100")]
        public int? ReleaseYear { get; set; }

        [Url(ErrorMessage = "Invalid image URL")]
        [MaxLength(500)]
        public string? CoverImage { get; set; }

        public List<int> GenreIds { get; set; } = new();


    }
}
