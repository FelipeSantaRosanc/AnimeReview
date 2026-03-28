using System.ComponentModel.DataAnnotations;

namespace AnimeReview.DTOs.User
{
    public class UpdateProfileDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        [Url]
        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }

    }
}
