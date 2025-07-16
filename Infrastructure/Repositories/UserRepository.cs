using MyWeatherApp.Infrastructure.Data;
using System.Data.Common;
using System.Data.SQLite;
using WeatherApp.Api.Core.Interfaces.Repositories;
using WeatherApp.Api.Core.Models;
using WeatherApp.Api.Infrastructure.Data;

namespace WeatherApp.Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public UserRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, Username, Email, PasswordHash, FirstName, LastName, 
                              Avatar, CreatedAt, UpdatedAt, IsActive 
                       FROM Users WHERE Id = @id AND IsActive = 1";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUserFromReader(reader);
            }
            return null;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, Username, Email, PasswordHash, FirstName, LastName, 
                        Avatar, CreatedAt, UpdatedAt, IsActive 
                        FROM Users WHERE Username = @username AND IsActive = 1";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUserFromReader(reader);
            }
            return null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"SELECT Id, Username, Email, PasswordHash, FirstName, LastName, 
                              Avatar, CreatedAt, UpdatedAt, IsActive 
                       FROM Users WHERE Email = @email AND IsActive = 1";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUserFromReader(reader);
            }
            return null;
        }

        public async Task<User> CreateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, 
                                         Avatar, CreatedAt, UpdatedAt, IsActive) 
                       VALUES (@username, @email, @passwordHash, @firstName, @lastName, 
                              @avatar, @createdAt, @updatedAt, @isActive);
                       SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@firstName", user.FirstName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@lastName", user.LastName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@avatar", user.Avatar ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@createdAt", user.CreatedAt);
            command.Parameters.AddWithValue("@updatedAt", user.UpdatedAt);
            command.Parameters.AddWithValue("@isActive", user.IsActive);

            var id = await command.ExecuteScalarAsync();
            user.Id = Convert.ToInt32(id);
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"UPDATE Users SET 
                              FirstName = @firstName, 
                              LastName = @lastName, 
                              Avatar = @avatar, 
                              UpdatedAt = @updatedAt
                       WHERE Id = @id";

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@firstName", user.FirstName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@lastName", user.LastName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@avatar", user.Avatar ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@updatedAt", user.UpdatedAt);

            await command.ExecuteNonQueryAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "UPDATE Users SET IsActive = 0 WHERE Id = @id";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = "SELECT COUNT(*) FROM Users WHERE Username = @username AND IsActive = 1";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);

            var count = await command.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }

        //private User MapUserFromReader(SQLiteDataReader reader)
        //{
        //    return new User
        //    {
        //        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //        Username = reader.GetString(reader.GetOrdinal("Username")),
        //        Email = reader.GetString(reader.GetOrdinal("Email")),
        //        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
        //        FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
        //        LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
        //        Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(reader.GetOrdinal("Avatar")),
        //        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        //        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
        //        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        //    };
        //}

        private User MapUserFromReader(DbDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(reader.GetOrdinal("Avatar")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}