//// Program.cs
//using System.Text;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using WeatherApp.Api.Middleware;
//using WeatherApp.Api.Services;
//using MyWeatherApp.Infrastructure.Data;
//using WeatherApp.Api.Extensions;

//var builder = WebApplication.CreateBuilder(args);

//// Add services
//builder.Services.AddControllers();
//builder.Services.AddApplicationServices();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// HttpClient
//builder.Services.AddHttpClient<WeatherService>();

//// CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//// SignalR
//builder.Services.AddSignalR();

//// JWT Authentication
//var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//var secretKey = jwtSettings["SecretKey"];
//var issuer = jwtSettings["Issuer"];
//var audience = jwtSettings["Audience"];

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//            ValidateIssuer = true,
//            ValidIssuer = issuer,
//            ValidateAudience = true,
//            ValidAudience = audience,
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero
//        };
//    });

//// Custom Services
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddSingleton(new SqliteConnectionFactory(connectionString));
//builder.Services.AddScoped<DatabaseInitializer>();
//builder.Services.AddScoped<TokenService>();
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<WeatherService>();
//builder.Services.AddScoped<ChatService>();

//var app = builder.Build();

//// Setup Render.com PORT (Render uses environment variable PORT)
//var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
//app.Urls.Add($"http://*:{port}");

//// Initialize Database
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
//    await dbContext.InitializeDatabaseAsync();
//}

//// Middlewares
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//// Optional: Remove if Render auto-handles HTTPS
//// app.UseHttpsRedirection();

//app.UseCors("AllowAll");
//app.UseMiddleware<JwtMiddleware>();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();
//app.MapHub<ChatHub>("/chatHub");

//// Health Check Endpoint (Optional)
//app.MapGet("/", () => "Weather App API is running on Render!");

//app.Run();





using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WeatherApp.Api.Extensions;
using MyWeatherApp.Infrastructure.Data; // Fixed namespace
using WeatherApp.Api.Middleware;
using WeatherApp.Api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather API",
        Version = "v1",
        Description = "A simple weather application API"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add application services (assuming this extension method exists)
builder.Services.AddApplicationServices();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// SignalR
builder.Services.AddSignalR();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

// Validate JWT configuration
if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
{
    throw new InvalidOperationException("JWT configuration is missing or incomplete in appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // For SignalR JWT support
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Database Services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DefaultConnection string is missing in appsettings.json");
}

// Register SqliteConnectionFactory with connection string
//builder.Services.AddSingleton(new SqliteConnectionFactory(connectionString));

builder.Services.AddSingleton<SqliteConnectionFactory>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");
    return new SqliteConnectionFactory(connectionString);
});
builder.Services.AddScoped<DatabaseInitializer>();

// Uncomment these services as you implement them
// builder.Services.AddScoped<TokenService>();
// builder.Services.AddScoped<AuthService>();
// builder.Services.AddScoped<WeatherService>();
// builder.Services.AddScoped<ChatService>();

var app = builder.Build();

// Setup Render.com PORT (Render uses environment variable PORT)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await dbInitializer.InitializeAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// Middleware pipeline order is important
app.UseCors("AllowAll");

// Custom JWT middleware (if needed)
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Hub mapping (uncomment when ChatHub is implemented)
// app.MapHub<ChatHub>("/chatHub");

// Health Check Endpoint
app.MapGet("/", () => "Weather App API is running on Render!");

app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

app.Run();