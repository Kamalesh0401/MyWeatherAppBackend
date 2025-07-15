using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Services;
using System.Security.Claims;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<ProfileResponse>> GetProfile()
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [HttpPut]
        public async Task<ActionResult<ProfileResponse>> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();
            var updatedProfile = await _profileService.UpdateProfileAsync(userId, request);

            if (updatedProfile == null)
                return NotFound();

            return Ok(updatedProfile);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            var success = await _profileService.ChangePasswordAsync(userId, request);

            if (!success)
                return BadRequest("Invalid current password");

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteProfile()
        {
            var userId = GetCurrentUserId();
            var success = await _profileService.DeleteProfileAsync(userId);

            if (!success)
                return NotFound();

            return Ok(new { message = "Profile deleted successfully" });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }
    }
}