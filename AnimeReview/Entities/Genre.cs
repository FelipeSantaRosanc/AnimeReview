namespace AnimeReview.Entities
{
    public class Genre
    {
        public int  Id { get; set; }

        public string Name { get; set; }

        public ICollection<AnimeGenre> AnimeGenres { get; set; }

    }
}
