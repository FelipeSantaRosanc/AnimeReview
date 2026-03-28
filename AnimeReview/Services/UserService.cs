using AnimeReview.DTOs.User;
using AnimeReview.Entities;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services.Interfaces;
using AnimeReview.Shared;
using Microsoft.AspNetCore.Identity;

namespace AnimeReview.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(string userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null)
            return Result<UserProfileDto>.NotFound("User not found");

        return Result<UserProfileDto>.Success(MapToDto(user));
    }

    public async Task<Result<UserProfileDto>> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null)
            return Result<UserProfileDto>.NotFound("User not found");

        user.Name = dto.Name ?? user.Name;
        user.PhoneNumber = dto.Phone ?? user.PhoneNumber;
        user.Address = dto.Address ?? user.Address;
        user.Bio = dto.Bio ?? user.Bio;
        user.ProfileImage = dto.ProfileImageUrl ?? user.ProfileImage;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<UserProfileDto>.Success(MapToDto(user));
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result.NotFound("User not found");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
            return Result.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result.Success();
    }

    private static UserProfileDto MapToDto(ApplicationUser user)
    {
        return new UserProfileDto
        {
            Name = user.Name,
            Phone = user.PhoneNumber,
            Address = user.Address,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImage
        };
    }
}