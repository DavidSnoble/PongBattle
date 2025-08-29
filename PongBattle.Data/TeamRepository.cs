using Dapper;
using PongBattle.Domain;

namespace PongBattle.Data;

public class TeamRepository : BaseDataRepository
{
    public Team? Get(int id)
    {
        var sql = @"
            SELECT t.Id, t.Name, t.PlayerOneId, t.PlayerTwoId,
                   u1.Id, u1.FirstName, u1.LastName, u1.EmailAddress, u1.PhoneNumber,
                   u2.Id, u2.FirstName, u2.LastName, u2.EmailAddress, u2.PhoneNumber
            FROM Teams t
            LEFT JOIN Users u1 ON t.PlayerOneId = u1.Id
            LEFT JOIN Users u2 ON t.PlayerTwoId = u2.Id
            WHERE t.Id = @id";

        var result = Connection.Query<Team, User?, User?, Team>(
            sql,
            (team, playerOne, playerTwo) =>
            {
                team.PlayerOne = playerOne;
                team.PlayerTwo = playerTwo;
                return team;
            },
            new { id },
            splitOn: "Id,Id"
        );

        return result.FirstOrDefault();
    }

    public IEnumerable<Team> GetAll()
    {
        var sql = @"
            SELECT t.Id, t.Name, t.PlayerOneId, t.PlayerTwoId,
                   u1.Id, u1.FirstName, u1.LastName, u1.EmailAddress, u1.PhoneNumber,
                   u2.Id, u2.FirstName, u2.LastName, u2.EmailAddress, u2.PhoneNumber
            FROM Teams t
            LEFT JOIN Users u1 ON t.PlayerOneId = u1.Id
            LEFT JOIN Users u2 ON t.PlayerTwoId = u2.Id";

        var result = Connection.Query<Team, User?, User?, Team>(
            sql,
            (team, playerOne, playerTwo) =>
            {
                team.PlayerOne = playerOne;
                team.PlayerTwo = playerTwo;
                return team;
            },
            splitOn: "Id,Id"
        );

        return result;
    }

    public int Create(Team team)
    {
        var sql = @"
            INSERT INTO Teams (Name, PlayerOneId, PlayerTwoId) 
            VALUES (@Name, @PlayerOneId, @PlayerTwoId);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        return Connection.QuerySingle<int>(sql, team);
    }

    public void Update(Team team)
    {
        var sql = @"
            UPDATE Teams SET 
                Name = @Name, 
                PlayerOneId = @PlayerOneId, 
                PlayerTwoId = @PlayerTwoId
            WHERE Id = @Id";
        
        Connection.Execute(sql, team);
    }
}
