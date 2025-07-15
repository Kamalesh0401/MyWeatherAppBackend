using MyWeatherApp.Infrastructure.Data;
using System.Data.Common;
using System.Data.SQLite;
using WeatherApp.Api.Core.Interfaces.Repositories;
using WeatherApp.Api.Core.Models;
using WeatherApp.Api.Infrastructure.Data;

namespace WeatherApp.Api.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public CommentRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, LocationName, Content, CreatedAt, UpdatedAt, IsActive 
                       FROM Comments WHERE Id = @id AND IsActive = 1";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapCommentFromReader(reader);
            }
            return null;
        }

        public async Task<List<Comment>> GetByLocationAsync(string location, int limit = 50, int offset = 0)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, LocationName, Content, CreatedAt, UpdatedAt, IsActive 
                       FROM Comments 
                       WHERE LocationName = @location AND IsActive = 1 
                       ORDER BY CreatedAt DESC 
                       LIMIT @limit OFFSET @offset";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@location", location);
            command.Parameters.AddWithValue("@limit", limit);
            command.Parameters.AddWithValue("@offset", offset);

            var comments = new List<Comment>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                comments.Add(MapCommentFromReader(reader));
            }
            return comments;
        }

        public async Task<List<Comment>> GetByUserIdAsync(int userId, int limit = 50, int offset = 0)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, UserId, Username, LocationName, Content, CreatedAt, UpdatedAt, IsActive 
                       FROM Comments 
                       WHERE UserId = @userId AND IsActive = 1 
                       ORDER BY CreatedAt DESC 
                       LIMIT @limit OFFSET @offset";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@limit", limit);
            command.Parameters.AddWithValue("@offset", offset);

            var comments = new List<Comment>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                comments.Add(MapCommentFromReader(reader));
            }
            return comments;
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"INSERT INTO Comments (UserId, Username, LocationName, Content, CreatedAt, UpdatedAt, IsActive) 
                       VALUES (@userId, @username, @locationName, @content, @createdAt, @updatedAt, @isActive);
                       SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", comment.UserId);
            command.Parameters.AddWithValue("@username", comment.Username);
            command.Parameters.AddWithValue("@locationName", comment.LocationName);
            command.Parameters.AddWithValue("@content", comment.Content);
            command.Parameters.AddWithValue("@createdAt", comment.CreatedAt);
            command.Parameters.AddWithValue("@updatedAt", comment.UpdatedAt);
            command.Parameters.AddWithValue("@isActive", comment.IsActive);

            var id = await command.ExecuteScalarAsync();
            comment.Id = Convert.ToInt32(id);
            return comment;
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"UPDATE Comments SET 
                              Content = @content, 
                              UpdatedAt = @updatedAt
                       WHERE Id = @id AND IsActive = 1";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", comment.Id);
            command.Parameters.AddWithValue("@content", comment.Content);
            command.Parameters.AddWithValue("@updatedAt", comment.UpdatedAt);

            await command.ExecuteNonQueryAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "UPDATE Comments SET IsActive = 0 WHERE Id = @id";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "SELECT COUNT(*) FROM Comments WHERE Id = @id AND IsActive = 1";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var count = await command.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }

        public async Task<int> GetCountByLocationAsync(string location)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "SELECT COUNT(*) FROM Comments WHERE LocationName = @location AND IsActive = 1";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@location", location);

            var count = await command.ExecuteScalarAsync();
            return Convert.ToInt32(count);
        }

        //private Comment MapCommentFromReader(SQLiteDataReader reader)
        //{
        //    return new Comment
        //    {
        //        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
        //        Username = reader.GetString(reader.GetOrdinal("Username")),
        //        LocationName = reader.GetString(reader.GetOrdinal("LocationName")),
        //        Content = reader.GetString(reader.GetOrdinal("Content")),
        //        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        //        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
        //        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        //    };
        //}

        private Comment MapCommentFromReader(DbDataReader reader)
        {
            return new Comment
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                LocationName = reader.GetString(reader.GetOrdinal("LocationName")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}