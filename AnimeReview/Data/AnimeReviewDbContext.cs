using AnimeReview.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Data
{
    public class AnimeReviewDbContext : IdentityDbContext<ApplicationUser>
    {
        public AnimeReviewDbContext(DbContextOptions<AnimeReviewDbContext> options)
            : base(options)
        {

        }


        public DbSet<Anime> Animes { get; set; }

        public DbSet<Manga> Mangas { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<AnimeGenre> AnimeGenres { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AnimeGenre>()
                .HasKey(ag => new { ag.AnimeId, ag.GenreId });

            modelBuilder.Entity<AnimeGenre>()
                .HasOne(ag => ag.Anime)
                .WithMany(a => a.AnimeGenres)
                .HasForeignKey(ag => ag.AnimeId);

            modelBuilder.Entity<AnimeGenre>()
                .HasOne(ag => ag.Genre)
                .WithMany(g => g.AnimeGenres)
                .HasForeignKey(ag => ag.GenreId);


        }

    }
}
