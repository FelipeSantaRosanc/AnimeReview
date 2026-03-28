using AnimeReview.Data;
using AnimeReview.Repositories.Interfaces;

namespace AnimeReview.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AnimeReviewDbContext _context;
    private bool _disposed;

    public IAnimeRepository Animes { get; }
    public IReviewRepository Reviews { get; }
    public IGenreRepository Genres { get; }
    public IUserRepository Users { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public UnitOfWork(AnimeReviewDbContext context)
    {
        _context = context;
        Animes = new AnimeRepository(context);
        Reviews = new ReviewRepository(context);
        Genres = new GenreRepository(context);
        Users = new UserRepository(context);
        RefreshTokens = new RefreshTokenRepository(context);
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}