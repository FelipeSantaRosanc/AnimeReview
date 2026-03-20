using Microsoft.EntityFrameworkCore;

namespace AnimeReview.Data
{
    public class AnimeReviewDbContextFactory
    {
        public AnimeReviewDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AnimeReviewDbContext>();

            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            );

            return new AnimeReviewDbContext(optionsBuilder.Options);
        }


    }
}
