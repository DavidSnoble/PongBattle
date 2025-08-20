namespace PongBattle.Data;

using Microsoft.Data.SqlClient;

public class DbContext
{
    public DbContext()
    {
        string? dbUser = Environment.GetEnvironmentVariable("PONG_BATTLE_DB_USER");
        string? dbPassword = Environment.GetEnvironmentVariable("PONG_BATTLE_DB_PASSWORD");
        var connectionString =
            $"Server=.;Initial Catalog=PongBattle;User ID={dbUser};Password={dbPassword};TrustServerCertificate=true";
        Connection = new SqlConnection(connectionString);
    }

    public SqlConnection Connection { get; }
}
