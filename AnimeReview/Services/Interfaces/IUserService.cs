using AnimeReview.DTOs.User;
using AnimeReview.Shared;

namespace AnimeReview.Services.Interfaces;

public interface IUserService
{
    Task<Result<UserProfileDto>> GetProfileAsync(string userId);
    Task<Result<UserProfileDto>> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto);
}