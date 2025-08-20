namespace PongBattle.Data;

using Dapper;
using PongBattle.Domain;

public class UserRepository
{
    public User? Get(int id)
    {
        var dbContext = new DbContext();
        var connection = dbContext.Connection;
        // Query data with Dapper
        var sql = "SELECT * FROM Users WHERE Id = @id";
        var user = connection.QueryFirstOrDefault<User>(sql, new { id = id });
        return user;
    }

    public IEnumerable<User> GetAll()
    {
        var dbContext = new DbContext();
        var connection = dbContext.Connection;
        // Query data with Dapper
        var sql = "SELECT * FROM Users";
        var users = connection.Query<User>(sql);
        return users;
    }
}
