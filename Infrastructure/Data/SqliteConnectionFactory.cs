using System.Data.SQLite;

namespace MyWeatherApp.Infrastructure.Data
{
    public class SqliteConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}