using AnimeReview.DTOs.User;
using AnimeReview.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AnimeReview.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        var result = await _userService.GetProfileAsync(userId);

        if (result.IsFailure)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var result = await _userService.UpdateProfileAsync(userId, dto);

        if (result.IsFailure)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var result = await _userService.ChangePasswordAsync(userId, dto);

        if (result.IsFailure)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(new { message = "Password changed successfully" });
    }
}