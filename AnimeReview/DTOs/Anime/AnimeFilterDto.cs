namespace AnimeReview.DTOs.Anime
{
    public class AnimeFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? GenreId { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public string? SortBy { get; set; } = "title"; // title, year, rating
        public bool SortDescending { get; set; } = false;

    }
}
