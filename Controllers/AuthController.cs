using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.DTOs;
using WeatherApp.Api.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Username and password are required"
                });
            }

            var (success, token, user) = await _authService.LoginAsync(request.Username, request.Password);

            if (!success)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                });
            }

            return Ok(new AuthResponse
            {
                Success = true,
                Token = token,
                Message = "Login successful",
                User = new UserInfo
                {
                    Id = user!.Id,
                    Username = user.Username,
                    Email = user.Email
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Username, email, and password are required"
                });
            }

            var (success, token, user) = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

            if (!success)
            {
                return Conflict(new AuthResponse
                {
                    Success = false,
                    Message = "Username already exists"
                });
            }

            return Ok(new AuthResponse
            {
                Success = true,
                Token = token,
                Message = "Registration successful",
                User = new UserInfo
                {
                    Id = user!.Id,
                    Username = user.Username,
                    Email = user.Email
                }
            });
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = HttpContext.Items["UserId"] as int?;
            var username = HttpContext.Items["Username"] as string;

            if (userId == null || username == null)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            return Ok(new { UserId = userId, Username = username });
        }
    }
}
