namespace AnimeReview.DTOs.Anime
{
    public class AnimeResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Synopsis { get; set; }
        public int? ReleaseYear { get; set; }
        public string? CoverImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Dados calculados
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Relacionamentos
        public List<GenreDto> Genres { get; set; } = new();
    }

    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
