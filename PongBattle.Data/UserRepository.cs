namespace PongBattle.Data;

using PongBattle.Domain;

public class UserRepository
{
    public User Get(int id)
    {
        var user = new User
        {
            EmailAddress = "dsnoble11@stackoverflow.com",
            FirstName = "Dustin",
            LastName = "Golf",
        };
        return user;
    }
}
