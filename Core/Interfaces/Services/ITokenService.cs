using WeatherApp.Api.Core.Models;
using System.Security.Claims;

namespace WeatherApp.Api.Core.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        bool IsTokenValid(string token);
    }
}