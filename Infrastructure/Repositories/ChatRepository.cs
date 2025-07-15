using MyWeatherApp.Infrastructure.Data;
using System.Data.Common;
using System.Data.SQLite;
using WeatherApp.Api.Core.Interfaces.Repositories;
using WeatherApp.Api.Core.Models;
using WeatherApp.Api.Infrastructure.Data;

namespace WeatherApp.Api.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public ChatRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ChatMessage?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, Message, CreatedAt, IsActive 
                       FROM ChatMessages WHERE Id = @id AND IsActive = 1";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapChatMessageFromReader(reader);
            }
            return null;
        }

        public async Task<List<ChatMessage>> GetRecentMessagesAsync(int limit = 50, int offset = 0)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, Message, CreatedAt, IsActive 
                       FROM ChatMessages 
                       WHERE IsActive = 1 
                       ORDER BY CreatedAt DESC 
                       LIMIT @limit OFFSET @offset";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@limit", limit);
            command.Parameters.AddWithValue("@offset", offset);

            var messages = new List<ChatMessage>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(MapChatMessageFromReader(reader));
            }
            return messages.OrderBy(m => m.CreatedAt).ToList();
        }

        public async Task<List<ChatMessage>> GetMessagesByUserIdAsync(int userId, int limit = 50, int offset = 0)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, Message, CreatedAt, IsActive 
                       FROM ChatMessages 
                       WHERE UserId = @userId AND IsActive = 1 
                       ORDER BY CreatedAt DESC 
                       LIMIT @limit OFFSET @offset";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@limit", limit);
            command.Parameters.AddWithValue("@offset", offset);

            var messages = new List<ChatMessage>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(MapChatMessageFromReader(reader));
            }
            return messages;
        }

        public async Task<ChatMessage> CreateAsync(ChatMessage message)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"INSERT INTO ChatMessages (UserId, Username, Message, CreatedAt, IsActive) 
                       VALUES (@userId, @username, @message, @createdAt, @isActive);
                       SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", message.UserId);
            command.Parameters.AddWithValue("@username", message.Username);
            command.Parameters.AddWithValue("@message", message.Message);
            command.Parameters.AddWithValue("@createdAt", message.CreatedAt);
            command.Parameters.AddWithValue("@isActive", message.IsActive);

            var id = await command.ExecuteScalarAsync();
            message.Id = Convert.ToInt32(id);
            return message;
        }

        public async Task<ChatMessage> UpdateAsync(ChatMessage message)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"UPDATE ChatMessages SET 
                              Message = @message
                       WHERE Id = @id AND IsActive = 1";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", message.Id);
            command.Parameters.AddWithValue("@message", message.Message);

            await command.ExecuteNonQueryAsync();
            return message;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "UPDATE ChatMessages SET IsActive = 0 WHERE Id = @id";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "SELECT COUNT(*) FROM ChatMessages WHERE Id = @id AND IsActive = 1";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var count = await command.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }

        public async Task<int> GetTotalMessageCountAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "SELECT COUNT(*) FROM ChatMessages WHERE IsActive = 1";
            using var command = new SQLiteCommand(sql, connection);

            var count = await command.ExecuteScalarAsync();
            return Convert.ToInt32(count);
        }

        //private ChatMessage MapChatMessageFromReader(SQLiteDataReader reader)
        //{
        //    return new ChatMessage
        //    {
        //        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
        //        Username = reader.GetString(reader.GetOrdinal("Username")),
        //        Message = reader.GetString(reader.GetOrdinal("Message")),
        //        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        //        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        //    };
        //}

        private ChatMessage MapChatMessageFromReader(DbDataReader reader)
        {
            return new ChatMessage
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Message = reader.GetString(reader.GetOrdinal("Message")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}