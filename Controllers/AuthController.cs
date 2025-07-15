using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] string refreshToken)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<ActionResult> RevokeToken([FromBody] string refreshToken)
        {
            var success = await _authService.RevokeTokenAsync(refreshToken);

            if (!success)
                return BadRequest("Failed to revoke token");

            return Ok(new { message = "Token revoked successfully" });
        }

        [HttpPost("validate")]
        public async Task<ActionResult<bool>> ValidateToken([FromBody] string token)
        {
            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(new { isValid });
        }
    }
}