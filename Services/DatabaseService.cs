using System.Data.SQLite;
using WeatherApp.Api.Data;
using WeatherApp.Api.Models;

namespace WeatherApp.Api.Services
{
    public class DatabaseService
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public DatabaseService(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // User operations
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "SELECT Id, Username, Email, PasswordHash, CreatedAt FROM Users WHERE Username = @username";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }
            return null;
        }

        public async Task<User?> CreateUserAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"INSERT INTO Users (Username, Email, PasswordHash, CreatedAt) 
                       VALUES (@username, @email, @passwordHash, @createdAt);
                       SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@createdAt", user.CreatedAt);

            var id = await command.ExecuteScalarAsync();
            if (id != null)
            {
                user.Id = Convert.ToInt32(id);
                return user;
            }
            return null;
        }

        // Comment operations
        public async Task<List<Comment>> GetCommentsByLocationAsync(string location)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, LocationName, Content, CreatedAt 
                       FROM Comments 
                       WHERE LocationName = @location 
                       ORDER BY CreatedAt DESC";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@location", location);

            var comments = new List<Comment>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                comments.Add(new Comment
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    LocationName = reader.GetString(reader.GetOrdinal("LocationName")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return comments;
        }

        public async Task<Comment?> CreateCommentAsync(Comment comment)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"INSERT INTO Comments (UserId, Username, LocationName, Content, CreatedAt) 
                       VALUES (@userId, @username, @locationName, @content, @createdAt);
                       SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", comment.UserId);
            command.Parameters.AddWithValue("@username", comment.Username);
            command.Parameters.AddWithValue("@locationName", comment.LocationName);
            command.Parameters.AddWithValue("@content", comment.Content);
            command.Parameters.AddWithValue("@createdAt", comment.CreatedAt);

            var id = await command.ExecuteScalarAsync();
            if (id != null)
            {
                comment.Id = Convert.ToInt32(id);
                return comment;
            }
            return null;
        }

        // Chat operations
        public async Task<List<ChatMessage>> GetRecentChatMessagesAsync(int limit = 50)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, Message, CreatedAt 
                       FROM ChatMessages 
                       ORDER BY CreatedAt DESC 
                       LIMIT @limit";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@limit", limit);

            var messages = new List<ChatMessage>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new ChatMessage
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Message = reader.GetString(reader.GetOrdinal("Id")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("Id"))
                });
            }
            return messages.OrderBy(m => m.CreatedAt).ToList();
        }

        public async Task<ChatMessage?> CreateChatMessageAsync(ChatMessage message)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"INSERT INTO ChatMessages (UserId, Username, Message, CreatedAt) 
                       VALUES (@userId, @username, @message, @createdAt);
                       SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", message.UserId);
            command.Parameters.AddWithValue("@username", message.Username);
            command.Parameters.AddWithValue("@message", message.Message);
            command.Parameters.AddWithValue("@createdAt", message.CreatedAt);

            var id = await command.ExecuteScalarAsync();
            if (id != null)
            {
                message.Id = Convert.ToInt32(id);
                return message;
            }
            return null;
        }
    }
}