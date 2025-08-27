using Dapper;
using PongBattle.Domain;

namespace PongBattle.Data;

public class UserRepository : BaseDataRepository
{
    public User? Get(int id)
    {
        var sql = "SELECT * FROM Users WHERE Id = @id";
        var user = Connection.QueryFirstOrDefault<User>(sql, new { id });
        return user;
    }

    public IEnumerable<User> GetAll()
    {
        var sql = "SELECT * FROM Users";
        var users = Connection.Query<User>(sql);
        return users;
    }

    public int Create(User user)
    {
        var sql =
            @"
                INSERT INTO Users (FirstName, LastName, EmailAddress, PhoneNumber) VALUES (@FirstName, @LastName, @EmailAddress, @PhoneNumber);
                SELECT CAST(SCOPE_IDENTITY() as int)";
        return Connection.QuerySingle<int>(sql, user);
    }

    public void Update(User user)
    {
        var sql =
            @"
            UPDATE Users SET 
            FirstName=@FirstName, 
            LastName=@LastName, 
            EmailAddress=@EmailAddress, 
            PhoneNumber=@PhoneNumber
            WHERE Id = @Id;
        ";
        Connection.Execute(sql, user);
    }
}