//using System.Data.SQLite;
//using WeatherApp.Api.Models;

//namespace MyWeatherApp.Infrastructure.Data
//{
//    public class DatabaseInitializer
//    {
//        private readonly SqliteConnectionFactory _connectionFactory;

//        public DatabaseInitializer(SqliteConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public async Task InitializeDatabaseAsync()
//        {
//            using var connection = _connectionFactory.CreateConnection();
//            await connection.OpenAsync();

//            var createUsersTable = @"
//                CREATE TABLE IF NOT EXISTS Users (
//                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
//                    Username TEXT UNIQUE NOT NULL,
//                    Email TEXT UNIQUE NOT NULL,
//                    PasswordHash TEXT NOT NULL,
//                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
//                )";

//            var createCommentsTable = @"
//                CREATE TABLE IF NOT EXISTS Comments (
//                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
//                    UserId INTEGER NOT NULL,
//                    Username TEXT NOT NULL,
//                    LocationName TEXT NOT NULL,
//                    Content TEXT NOT NULL,
//                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
//                    FOREIGN KEY (UserId) REFERENCES Users(Id)
//                )";

//            var createChatMessagesTable = @"
//                CREATE TABLE IF NOT EXISTS ChatMessages (
//                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
//                    UserId INTEGER NOT NULL,
//                    Username TEXT NOT NULL,
//                    Message TEXT NOT NULL,
//                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
//                    FOREIGN KEY (UserId) REFERENCES Users(Id)
//                )";

//            var createWeatherCacheTable = @"
//                CREATE TABLE IF NOT EXISTS WeatherCache (
//                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
//                    LocationName TEXT NOT NULL,
//                    WeatherData TEXT NOT NULL,
//                    CachedAt DATETIME DEFAULT CURRENT_TIMESTAMP
//                )";

//            await ExecuteCommandAsync(connection, createUsersTable);
//            await ExecuteCommandAsync(connection, createCommentsTable);
//            await ExecuteCommandAsync(connection, createChatMessagesTable);
//            await ExecuteCommandAsync(connection, createWeatherCacheTable);
//        }

//        private async Task ExecuteCommandAsync(SQLiteConnection connection, string sql)
//        {
//            using var command = new SQLiteCommand(sql, connection);
//            await command.ExecuteNonQueryAsync();
//        }
//    }
//}


using MyWeatherApp.Infrastructure.Data;
using System.Data.SQLite;

namespace WeatherApp.Api.Infrastructure.Data
{
    public class DatabaseInitializer
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public DatabaseInitializer(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await CreateTablesAsync(connection);
        }

        private async Task CreateTablesAsync(SQLiteConnection connection)
        {
            // Create Users table
            var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    Email TEXT UNIQUE NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    FirstName TEXT,
                    LastName TEXT,
                    Avatar TEXT,
                    CreatedAt DATETIME NOT NULL,
                    UpdatedAt DATETIME NOT NULL,
                    IsActive BOOLEAN NOT NULL DEFAULT 1
                )";

            using var command1 = new SQLiteCommand(createUsersTable, connection);
            await command1.ExecuteNonQueryAsync();

            // Create Comments table
            var createCommentsTable = @"
                CREATE TABLE IF NOT EXISTS Comments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Username TEXT NOT NULL,
                    LocationName TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    CreatedAt DATETIME NOT NULL,
                    UpdatedAt DATETIME NOT NULL,
                    IsActive BOOLEAN NOT NULL DEFAULT 1,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )";

            using var command2 = new SQLiteCommand(createCommentsTable, connection);
            await command2.ExecuteNonQueryAsync();

            // Create ChatMessages table
            var createChatMessagesTable = @"
                CREATE TABLE IF NOT EXISTS ChatMessages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Username TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    CreatedAt DATETIME NOT NULL,
                    IsActive BOOLEAN NOT NULL DEFAULT 1,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )";

            using var command3 = new SQLiteCommand(createChatMessagesTable, connection);
            await command3.ExecuteNonQueryAsync();

            // Create indexes
            await CreateIndexesAsync(connection);
        }

        private async Task CreateIndexesAsync(SQLiteConnection connection)
        {
            var indexes = new[]
            {
                "CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username)",
                "CREATE INDEX IF NOT EXISTS idx_users_email ON Users(Email)",
                "CREATE INDEX IF NOT EXISTS idx_comments_location ON Comments(LocationName)",
                "CREATE INDEX IF NOT EXISTS idx_comments_user ON Comments(UserId)",
                "CREATE INDEX IF NOT EXISTS idx_chatmessages_user ON ChatMessages(UserId)",
                "CREATE INDEX IF NOT EXISTS idx_chatmessages_created ON ChatMessages(CreatedAt)"
            };

            foreach (var index in indexes)
            {
                using var command = new SQLiteCommand(index, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
