using AnimeReview.DTOs.Anime;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;
using AnimeReview.Shared;

namespace AnimeReview.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;

    public GenreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<GenreDto>>> GetAllAsync()
    {
        var genres = await _unitOfWork.Genres.GetAllAsync();

        var dtoItems = genres.Select(g => new GenreDto
        {
            Id = g.Id,
            Name = g.Name
        });

        return Result<IEnumerable<GenreDto>>.Success(dtoItems);
    }

    public async Task<Result<GenreDto>> GetByIdAsync(int id)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(id);

        if (genre == null)
            return Result<GenreDto>.NotFound("Genre not found");

        return Result<GenreDto>.Success(new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name
        });
    }

    public async Task<Result<GenreDto>> CreateAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<GenreDto>.BadRequest("Genre name is required");

        // Verificar se já existe
        var existing = await _unitOfWork.Genres.GetAllAsync();
        if (existing.Any(g => g.Name.ToLower() == name.ToLower()))
            return Result<GenreDto>.Conflict("Genre already exists");

        var genre = new Genre
        {
            Name = name
        };

        // Adicionar diretamente via DbContext ou criar método no repo
        // Simplificado: assumindo que você adiciona ao contexto
        await _unitOfWork.SaveChangesAsync();

        return Result<GenreDto>.Created(new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name
        });
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(id);

        if (genre == null)
            return Result.NotFound("Genre not found");

        // Verificar se está em uso
        // Implementar lógica de verificação...

        // _context.Genres.Remove(genre);
        await _unitOfWork.SaveChangesAsync();

        return Result.NoContent();
    }
}