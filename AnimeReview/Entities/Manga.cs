using System.ComponentModel.DataAnnotations;

namespace AnimeReview.Entities
{
    public class Manga
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Synopsis { get; set; }

        public string? CoverImage { get; set; }

        public int ReleaseYear { get; set; }

        public ICollection<Review>? Reviews { get; set; }


    }
}
