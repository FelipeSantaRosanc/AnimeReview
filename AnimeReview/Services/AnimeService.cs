using AnimeReview.DTOs.Anime;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;
using AnimeReview.Shared;

namespace AnimeReview.Services
{
    public class AnimeService : IAnimeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnimeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<AnimeResponseDto>>> GetAllAsync(
            PaginationParams pagination,
            AnimeFilterDto? filter = null)
        {
            var result = await _unitOfWork.Animes.GetAllAsync(pagination, filter);

            var dtoItems = result.Items.Select(MapToDto).ToList();

            return Result<PagedResult<AnimeResponseDto>>.Success(new PagedResult<AnimeResponseDto>
            {
                Items = dtoItems,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            });
        }

        public async Task<Result<AnimeResponseDto>> GetByIdAsync(int id)
        {
            var anime = await _unitOfWork.Animes.GetByIdWithDetailsAsync(id);

            if (anime == null)
                return Result<AnimeResponseDto>.NotFound("Anime not found");

            return Result<AnimeResponseDto>.Success(MapToDto(anime));
        }

        public async Task<Result<AnimeResponseDto>> CreateAsync(AnimeCreateDto dto)
        {
            // Verificar duplicata
            if (await _unitOfWork.Animes.ExistsByTitleAsync(dto.Title))
                return Result<AnimeResponseDto>.Conflict("An anime with this title already exists");

            // Validar gêneros
            if (dto.GenreIds.Any())
            {
                var existingGenres = await _unitOfWork.Genres.GetByIdsAsync(dto.GenreIds);
                if (existingGenres.Count != dto.GenreIds.Count)
                    return Result<AnimeResponseDto>.BadRequest("One or more genres not found");
            }

            var anime = new Anime
            {
                Title = dto.Title,
                Synopsis = dto.Synopsis,
                ReleaseYear = dto.ReleaseYear,
                CoverImage = dto.CoverImage,
                AnimeGenres = dto.GenreIds.Select(gid => new AnimeGenre
                {
                    GenreId = gid
                }).ToList()
            };

            await _unitOfWork.Animes.AddAsync(anime);
            await _unitOfWork.SaveChangesAsync();

            // Recarregar com relacionamentos para retornar completo
            var createdAnime = await _unitOfWork.Animes.GetByIdWithDetailsAsync(anime.Id);
            return Result<AnimeResponseDto>.Created(MapToDto(createdAnime!));
        }

        public async Task<Result<AnimeResponseDto>> UpdateAsync(int id, AnimeUpdateDto dto)
        {
            var anime = await _unitOfWork.Animes.GetByIdWithDetailsAsync(id);

            if (anime == null)
                return Result<AnimeResponseDto>.NotFound("Anime not found");

            // Verificar duplicata de título (exceto este anime)
            var existingWithTitle = await _unitOfWork.Animes.GetByIdAsync(id); // Só para exemplo
                                                                               // Aqui você implementaria: buscar por título e verificar se ID é diferente

            // Validar gêneros
            if (dto.GenreIds.Any())
            {
                var existingGenres = await _unitOfWork.Genres.GetByIdsAsync(dto.GenreIds);
                if (existingGenres.Count != dto.GenreIds.Distinct().Count())
                    return Result<AnimeResponseDto>.BadRequest("One or more genres not found");
            }

            // Atualizar dados
            anime.Title = dto.Title;
            anime.Synopsis = dto.Synopsis;
            anime.ReleaseYear = dto.ReleaseYear;
            anime.CoverImage = dto.CoverImage;
            anime.UpdatedAt = DateTime.UtcNow;

            // Atualizar gêneros (estratégia: remover todos e adicionar novos)
            anime.AnimeGenres.Clear();
            foreach (var genreId in dto.GenreIds.Distinct())
            {
                anime.AnimeGenres.Add(new AnimeGenre
                {
                    AnimeId = anime.Id,
                    GenreId = genreId
                });
            }

            await _unitOfWork.Animes.UpdateAsync(anime);
            await _unitOfWork.SaveChangesAsync();

            // Recarregar para retornar dados atualizados
            var updatedAnime = await _unitOfWork.Animes.GetByIdWithDetailsAsync(anime.Id);
            return Result<AnimeResponseDto>.Success(MapToDto(updatedAnime!));
        }

        public async Task<Result> DeleteAsync(int id)
        {
            if (!await _unitOfWork.Animes.ExistsAsync(id))
                return Result.NotFound("Anime not found");

            await _unitOfWork.Animes.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Result.NoContent();
        }

        private static AnimeResponseDto MapToDto(Anime anime)
        {
            var reviews = anime.Reviews?.Where(r => !r.IsDeleted).ToList() ?? new List<Review>();

            return new AnimeResponseDto
            {
                Id = anime.Id,
                Title = anime.Title,
                Synopsis = anime.Synopsis,
                ReleaseYear = anime.ReleaseYear,
                CoverImage = anime.CoverImage,
                CreatedAt = anime.CreatedAt,
                UpdatedAt = anime.UpdatedAt,
                AverageRating = reviews.Any() ? reviews.Average(r => r.Score) : 0,
                TotalReviews = reviews.Count,
                Genres = anime.AnimeGenres?.Select(ag => new GenreDto
                {
                    Id = ag.Genre.Id,
                    Name = ag.Genre.Name
                }).ToList() ?? new List<GenreDto>()
            };
        }
    }
}
