using AnimeReview.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Data;

public class AnimeReviewDbContext : IdentityDbContext<ApplicationUser>
{
    public AnimeReviewDbContext(DbContextOptions<AnimeReviewDbContext> options)
        : base(options)
    {
    }

    public DbSet<Anime> Animes { get; set; } = null!;
    public DbSet<Manga> Mangas { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Favorite> Favorites { get; set; } = null!;
    public DbSet<AnimeGenre> AnimeGenres { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Soft delete global filter
        modelBuilder.Entity<Anime>().HasQueryFilter(a => !a.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);

        // AnimeGenre composite key
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

        // RefreshToken indexes
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.UserId);

        // Review indexes
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.AnimeId, r.IsDeleted });

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.AnimeId });
    }
}