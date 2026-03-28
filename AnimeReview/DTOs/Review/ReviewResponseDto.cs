namespace AnimeReview.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Dados do usuário (anônimo ou não)
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }

        // Dados do anime
        public int AnimeId { get; set; }
        public string AnimeTitle { get; set; } = string.Empty;

    }
}
