//using WeatherApp.Api.Core.DTOs;
//using WeatherApp.Api.Core.Interfaces.Repositories;
//using WeatherApp.Api.Core.Interfaces.Services;
//using WeatherApp.Api.Core.Models;
//using BCrypt.Net;
//using MyWeatherApp.Services.Interfaces;

//namespace WeatherApp.Api.Infrastructure.Services
//{
//    public class AuthService : IAuthService
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly ITokenService _tokenService;

//        public AuthService(IUserRepository userRepository, ITokenService tokenService)
//        {
//            _userRepository = userRepository;
//            _tokenService = tokenService;
//        }

//        public async Task<AuthResponse> LoginAsync(LoginRequest request)
//        {
//            var user = await _userRepository.GetByUsernameAsync(request.Username);
//            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//            {
//                return new AuthResponse
//                {
//                    Success = false,
//                    Message = "Invalid username or password"
//                };
//            }

//            var token = _tokenService.GenerateAccessToken(user);
//            var refreshToken = _tokenService.GenerateRefreshToken();

//            return new AuthResponse
//            {
//                Success = true,
//                Token = token,
//                RefreshToken = refreshToken,
//                User = new ProfileResponse
//                {
//                    Id = user.Id,
//                    Username = user.Username,
//                    Email = user.Email,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    Avatar = user.Avatar,
//                    CreatedAt = user.CreatedAt,
//                    UpdatedAt = user.UpdatedAt
//                }
//            };
//        }

//        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
//        {
//            if (await _userRepository.ExistsAsync(request.Username))
//            {
//                return new AuthResponse
//                {
//                    Success = false,
//                    Message = "Username already exists"
//                };
//            }

//            var user = new User
//            {
//                Username = request.Username,
//                Email = request.Email,
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow,
//                IsActive = true
//            };

//            var createdUser = await _userRepository.CreateAsync(user);
//            var token = _tokenService.GenerateAccessToken(createdUser);
//            var refreshToken = _tokenService.GenerateRefreshToken();

//            return new AuthResponse
//            {
//                Success = true,
//                Token = token,
//                RefreshToken = refreshToken,
//                User = new ProfileResponse
//                {
//                    Id = createdUser.Id,
//                    Username = createdUser.Username,
//                    Email = createdUser.Email,
//                    FirstName = createdUser.FirstName,
//                    LastName = createdUser.LastName,
//                    Avatar = createdUser.Avatar,
//                    CreatedAt = createdUser.CreatedAt,
//                    UpdatedAt = createdUser.UpdatedAt
//                }
//            };
//        }
//    }
//}


using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Repositories;
using WeatherApp.Api.Core.Interfaces.Services;
using WeatherApp.Api.Core.Models;
using BCrypt.Net;

namespace WeatherApp.Api.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly Dictionary<string, (int UserId, DateTime ExpiresAt)> _refreshTokens;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
            _refreshTokens = new Dictionary<string, (int, DateTime)>();
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(request.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                var token = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Store refresh token (in production, use database)
                _refreshTokens[refreshToken] = (user.Id, DateTime.UtcNow.AddDays(7));

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    User = MapToProfileResponse(user),
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (await _userRepository.ExistsAsync(request.Username))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdUser = await _userRepository.CreateAsync(user);
                var token = _tokenService.GenerateAccessToken(createdUser);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Store refresh token
                _refreshTokens[refreshToken] = (createdUser.Id, DateTime.UtcNow.AddDays(7));

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    User = MapToProfileResponse(createdUser),
                    Message = "Registration successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", request.Username);
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (!_refreshTokens.ContainsKey(refreshToken))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    };
                }

                var (userId, expiresAt) = _refreshTokens[refreshToken];
                if (DateTime.UtcNow > expiresAt)
                {
                    _refreshTokens.Remove(refreshToken);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Refresh token expired"
                    };
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var newToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                // Remove old refresh token and add new one
                _refreshTokens.Remove(refreshToken);
                _refreshTokens[newRefreshToken] = (user.Id, DateTime.UtcNow.AddDays(7));

                return new AuthResponse
                {
                    Success = true,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    User = MapToProfileResponse(user),
                    Message = "Token refreshed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred while refreshing token"
                };
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                return _refreshTokens.Remove(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                return false;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                return _tokenService.IsTokenValid(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }

        private ProfileResponse MapToProfileResponse(User user)
        {
            return new ProfileResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}