namespace AnimeReview.Entities
{
    public class Favorite
    {

        public int Id { get; set; }
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public int AnimeId { get; set; }

        public Anime? Anime { get; set; }

    }
}
