using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace MoonWorkRAPI.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MySqlConnection");
        }

        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}
