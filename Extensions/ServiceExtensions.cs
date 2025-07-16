//using MyWeatherApp.Infrastructure.Data;
//using WeatherApp.Api.Core.Interfaces.Repositories;
//using WeatherApp.Api.Core.Interfaces.Services;
//using WeatherApp.Api.Infrastructure.Repositories;
//using WeatherApp.Api.Infrastructure.Services;

//namespace WeatherApp.Api.Extensions
//{
//    public static class ServiceExtensions
//    {
//        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
//        {
//            // Database
//            //services.AddSingleton<SqliteConnectionFactory>();

//            // Repositories
//            services.AddScoped<IUserRepository, UserRepository>();
//            services.AddScoped<ICommentRepository, CommentRepository>();
//            services.AddScoped<IChatRepository, ChatRepository>();

//            // Services
//            services.AddScoped<IAuthService, AuthService>();
//            services.AddScoped<IProfileService, ProfileService>();
//            services.AddScoped<ITokenService, TokenService>();
//            services.AddScoped<IWeatherService, WeatherService>();
//            services.AddScoped<IChatService, ChatService>();

//            // HTTP Client for Weather Service
//            services.AddHttpClient<IWeatherService, WeatherService>();

//            return services;
//        }
//    }
//}


using MyWeatherApp.Infrastructure.Data;
using WeatherApp.Api.Core.Interfaces.Repositories;
using WeatherApp.Api.Core.Interfaces.Services;
using WeatherApp.Api.Infrastructure.Repositories;
using WeatherApp.Api.Infrastructure.Services;

namespace WeatherApp.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<IChatService, ChatService>();

            //HTTP Client for Weather Service
            services.AddHttpClient<IWeatherService, WeatherService>();

            return services;
        }
    }
}