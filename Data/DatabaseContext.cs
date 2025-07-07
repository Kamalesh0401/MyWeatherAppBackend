using System.Data.SQLite;
using WeatherApp.Api.Models;

namespace WeatherApp.Api.Data
{
    public class DatabaseContext
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public DatabaseContext(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeDatabaseAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    Email TEXT UNIQUE NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            var createCommentsTable = @"
                CREATE TABLE IF NOT EXISTS Comments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Username TEXT NOT NULL,
                    LocationName TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )";

            var createChatMessagesTable = @"
                CREATE TABLE IF NOT EXISTS ChatMessages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Username TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )";

            var createWeatherCacheTable = @"
                CREATE TABLE IF NOT EXISTS WeatherCache (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LocationName TEXT NOT NULL,
                    WeatherData TEXT NOT NULL,
                    CachedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            await ExecuteCommandAsync(connection, createUsersTable);
            await ExecuteCommandAsync(connection, createCommentsTable);
            await ExecuteCommandAsync(connection, createChatMessagesTable);
            await ExecuteCommandAsync(connection, createWeatherCacheTable);
        }

        private async Task ExecuteCommandAsync(SQLiteConnection connection, string sql)
        {
            using var command = new SQLiteCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
