using System.ComponentModel.DataAnnotations;

namespace AnimeReview.DTOs.Anime
{
    public class AnimeCreateDto
    {

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        public string Synopsis { get; set; }

        public int ReleaseYear { get; set; }

        public string ImageUrl { get; set; }


    }
}
