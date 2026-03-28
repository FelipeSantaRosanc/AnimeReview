namespace AnimeReview.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAnimeRepository Animes { get; }
        IReviewRepository Reviews { get; }
        IGenreRepository Genres { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

    }
}
