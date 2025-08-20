namespace PongBattle.Web.Models;

using PongBattle.Domain;

public class UserViewModel
{
    //public UserViewModel(User user)
    //{
    //    EmailAddress = user.EmailAddress;
    //    FirstName = user.FirstName;
    //    LastName = user.LastName;
    //}

    public static UserViewModel FromUser(User user)
    {
        return new UserViewModel
        {
            EmailAddress = user.EmailAddress,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }

    public required string EmailAddress { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
