namespace PongBattle.Data;

using Dapper;
using Domain;

public class TeamRepository : BaseDataRepository
{
    public Team? Get(int id)
    {
        var sql = "SELECT Id, Name, PlayerOneId, PlayerTwoId FROM Teams WHERE Id = @id";
        return Connection.QueryFirstOrDefault<Team>(sql, new { id });
    }

    public IEnumerable<Team> GetAll()
    {
        var sql = "SELECT Id, Name, PlayerOneId, PlayerTwoId FROM Teams";
        return Connection.Query<Team>(sql);
    }

    public int Create(Team team)
    {
        var sql =
            @"
        INSERT INTO Teams (Name, PlayerOneId, PlayerTwoId) VALUES (@Name, @PlayerOneId, @PlayerTwoId);
      SELECT CAST(SCOPE_IDENTITY() as int)";

        return Connection.QuerySingle<int>(sql, team);
    }

    public void Update(Team team)
    {
        var sql =
            @"
        UPDATE Team SET 
        Name = @Name,
        PlayerOneId = @PlayerOneId,
        PlayerTwoId = @PlayerTwoId
        WHERE Id = @Id;
        ";

        Connection.Execute(sql, team);
    }
}
