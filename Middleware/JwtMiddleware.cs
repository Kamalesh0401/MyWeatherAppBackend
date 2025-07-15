using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WeatherApp.Api.Core.Interfaces.Services;

namespace WeatherApp.Api.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var key = Encoding.UTF8.GetBytes(secretKey);

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

                context.Items["UserId"] = userId;
                context.Items["Username"] = username;
            }
            catch
            {
                // Invalid token
            }
        }
    }
}



//using System.Text;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using WeatherApp.Api.Core.Interfaces.Services;

//namespace WeatherApp.Api.Middleware
//{
//    public class JwtMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly IConfiguration _configuration;

//        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
//        {
//            _next = next;
//            _configuration = configuration;
//        }

//        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
//        {
//            var token = context.Request.Headers["Authorization"]
//                .FirstOrDefault()?.Split(" ").Last();

//            if (token != null)
//            {
//                await AttachUserToContext(context, tokenService, token);
//            }

//            await _next(context);
//        }

//        private async Task AttachUserToContext(HttpContext context, ITokenService tokenService, string token)
//        {
//            try
//            {
//                var principal = tokenService.ValidateToken(token);
//                if (principal != null)
//                {
//                    context.User = principal;
//                }
//            }
//            catch
//            {
//                // Token validation failed
//            }
//        }
//    }
//}