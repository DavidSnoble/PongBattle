namespace PongBattle.Data;

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using PongBattle.Domain;

public class UserRepository
{
    public User? Get(int id)
    {
        string dbUser = Environment.GetEnvironmentVariable("PONG_BATTLE_DB_USER");
        string dbPassword = Environment.GetEnvironmentVariable("PONG_BATTLE_DB_PASSWORD");
        var connectionString =
            $"Server=.;Initial Catalog=PongBattle;User ID={dbUser};Password={dbPassword};TrustServerCertificate=true";
        // Connect to the database
        var connection = new SqlConnection(connectionString);
        // Query data with Dapper
        var sql = "SELECT * FROM Users WHERE Id = @id";
        var user = connection.QueryFirstOrDefault<User>(sql, new { id = id });
        return user;
    }
}
