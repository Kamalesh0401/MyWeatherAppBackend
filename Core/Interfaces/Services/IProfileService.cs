using WeatherApp.Api.Core.DTOs;

namespace WeatherApp.Api.Core.Interfaces.Services
{
    public interface IProfileService
    {
        Task<ProfileResponse?> GetProfileAsync(int userId);
        Task<ProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<bool> DeleteProfileAsync(int userId);
    }
}