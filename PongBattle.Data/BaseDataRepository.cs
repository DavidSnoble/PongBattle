using Microsoft.Data.SqlClient;

namespace PongBattle.Data;

public abstract class BaseDataRepository
{
    protected static SqlConnection Connection
    {
        get
        {
            var dbContext = new DbContext();
            return dbContext.Connection;
        }
    }
}