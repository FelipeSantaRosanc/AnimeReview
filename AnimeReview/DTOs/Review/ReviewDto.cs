namespace AnimeReview.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }

        public int AnimeId { get; set; }

    }
}
