using AnimeReview.DTOs.Review;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;
using AnimeReview.Shared;

namespace AnimeReview.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ReviewResponseDto>>> GetByAnimeIdAsync(
        int animeId,
        PaginationParams pagination)
    {
        // Verificar se anime existe
        if (!await _unitOfWork.Animes.ExistsAsync(animeId))
            return Result<PagedResult<ReviewResponseDto>>.NotFound("Anime not found");

        var result = await _unitOfWork.Reviews.GetByAnimeIdAsync(animeId, pagination);

        var dtoItems = result.Items.Select(MapToDto).ToList();

        return Result<PagedResult<ReviewResponseDto>>.Success(new PagedResult<ReviewResponseDto>
        {
            Items = dtoItems,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        });
    }

    public async Task<Result<ReviewResponseDto>> GetByIdAsync(int id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);

        if (review == null)
            return Result<ReviewResponseDto>.NotFound("Review not found");

        return Result<ReviewResponseDto>.Success(MapToDto(review));
    }

    public async Task<Result<ReviewResponseDto>> CreateAsync(ReviewCreateDto dto, string userId)
    {
        // Verificar se anime existe
        if (!await _unitOfWork.Animes.ExistsAsync(dto.AnimeId))
            return Result<ReviewResponseDto>.NotFound("Anime not found");

        // Verificar se usuário já avaliou este anime
        if (await _unitOfWork.Reviews.UserHasReviewedAsync(userId, dto.AnimeId))
            return Result<ReviewResponseDto>.Conflict("You have already reviewed this anime");

        // Usar transação para garantir consistência
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var review = new Review
            {
                AnimeId = dto.AnimeId,
                UserId = userId,
                Score = dto.Score,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Atualizar média do anime
            await UpdateAnimeAverageRatingAsync(dto.AnimeId);

            await _unitOfWork.CommitAsync();

            // Recarregar com relacionamentos
            var createdReview = await _unitOfWork.Reviews.GetByIdAsync(review.Id);
            return Result<ReviewResponseDto>.Created(MapToDto(createdReview!));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<ReviewResponseDto>> UpdateAsync(int id, ReviewUpdateDto dto, string userId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);

        if (review == null)
            return Result<ReviewResponseDto>.NotFound("Review not found");

        // Verificar se é o dono da review
        if (review.UserId != userId)
            return Result<ReviewResponseDto>.Unauthorized("You can only update your own reviews");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            review.Score = dto.Score;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Recalcular média do anime
            await UpdateAnimeAverageRatingAsync(review.AnimeId);

            await _unitOfWork.CommitAsync();

            var updatedReview = await _unitOfWork.Reviews.GetByIdAsync(review.Id);
            return Result<ReviewResponseDto>.Success(MapToDto(updatedReview!));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result> DeleteAsync(int id, string userId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);

        if (review == null)
            return Result.NotFound("Review not found");

        // Verificar se é o dono da review
        if (review.UserId != userId)
            return Result.Unauthorized("You can only delete your own reviews");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var animeId = review.AnimeId;

            await _unitOfWork.Reviews.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            // Recalcular média
            await UpdateAnimeAverageRatingAsync(animeId);

            await _unitOfWork.CommitAsync();

            return Result.NoContent();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result> DeleteAsAdminAsync(int id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);

        if (review == null)
            return Result.NotFound("Review not found");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var animeId = review.AnimeId;

            await _unitOfWork.Reviews.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            await UpdateAnimeAverageRatingAsync(animeId);

            await _unitOfWork.CommitAsync();

            return Result.NoContent();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task UpdateAnimeAverageRatingAsync(int animeId)
    {
        var anime = await _unitOfWork.Animes.GetByIdWithDetailsAsync(animeId);
        if (anime != null)
        {
            // A média é calculada no mapping, mas aqui você poderia 
            // adicionar um campo AverageRating no Anime e atualizar
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private static ReviewResponseDto MapToDto(Review review)
    {
        return new ReviewResponseDto
        {
            Id = review.Id,
            Score = review.Score,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            UserName = review.User?.Name ?? review.User?.UserName ?? "Unknown",
            UserProfileImage = review.User?.ProfileImage,
            AnimeId = review.AnimeId,
            AnimeTitle = review.Anime?.Title ?? "Unknown"
        };
    }
}