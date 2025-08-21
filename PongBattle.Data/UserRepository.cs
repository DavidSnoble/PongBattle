using Dapper;
using PongBattle.Domain;

namespace PongBattle.Data;

public class UserRepository
{
    public User? Get(int id)
    {
        var dbContext = new DbContext();
        var connection = dbContext.Connection;
        // Query data with Dapper
        var sql = "SELECT * FROM Users WHERE Id = @id";
        var user = connection.QueryFirstOrDefault<User>(sql, new { id });
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

    public void Create(User user)
    {
        var dbContext = new DbContext();
        var connection = dbContext.Connection;
        var sql =
            @"
                INSERT INTO Users (FirstName, LastName, EmailAddress, PhoneNumber) VALUES (@FirstName, @LastName, @EmailAddress, @PhoneNumber);
                SELECT CAST(SCOPE_IDENTITY() as int)";
        connection.QuerySingle<int>(sql, user);
    }
}