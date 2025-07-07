using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WeatherApp.Api.Models;

namespace WeatherApp.Api.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseService _databaseService;

        public AuthService(IConfiguration configuration, DatabaseService databaseService)
        {
            _configuration = configuration;
            _databaseService = databaseService;
        }

        public async Task<(bool Success, string Token, User? User)> LoginAsync(string username, string password)
        {
            var user = await _databaseService.GetUserByUsernameAsync(username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return (false, string.Empty, null);
            }

            var token = GenerateJwtToken(user);
            return (true, token, user);
        }

        public async Task<(bool Success, string Token, User? User)> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _databaseService.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                return (false, string.Empty, null);
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _databaseService.CreateUserAsync(user);
            if (createdUser == null)
            {
                return (false, string.Empty, null);
            }

            var token = GenerateJwtToken(createdUser);
            return (true, token, createdUser);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            // Simple hashing for demo - use BCrypt or similar in production
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password + "salt"));
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}