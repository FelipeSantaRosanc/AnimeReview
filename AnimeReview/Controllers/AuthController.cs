using AnimeReview.DTOs.Auth;
using AnimeReview.Entities;
using AnimeReview.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AnimeReview.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableRateLimiting("Global")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("register")]
        [EnableRateLimiting("Login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { error = "Email already registered" });

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            await _userManager.AddToRoleAsync(user, "User");

            // Auto-login
            var authResult = await _authService.LoginAsync(dto.Email, dto.Password);

            if (authResult.IsFailure)
                return StatusCode(authResult.StatusCode, new { error = authResult.Error });

            return CreatedAtAction(nameof(Register), authResult.Value);
        }

        [HttpPost("login")]
        [EnableRateLimiting("Login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto.Email, dto.Password);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("Authenticated")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RefreshTokenAsync(dto.Token, dto.RefreshToken);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost("logout")]
        [Authorize]
        [EnableRateLimiting("Authenticated")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { error = "User not authenticated" });

            var result = await _authService.LogoutAsync(userId);

            if (result.IsFailure)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(new { message = "Logged out successfully" });
        }
    }
}