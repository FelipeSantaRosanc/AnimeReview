namespace AnimeReview.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public int AnimeId { get; set; }

        public Anime? Anime { get; set; }
        public int Score { get; internal set; }
    }
}
