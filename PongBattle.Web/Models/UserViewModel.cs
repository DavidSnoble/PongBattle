using PongBattle.Domain;

namespace PongBattle.Web.Models;

public class UserViewModel
{
    public required string EmailAddress { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? PhoneNumber { get; set; }

    public static UserViewModel FromUser(User user)
    {
        return new UserViewModel
        {
            EmailAddress = user.EmailAddress,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
    }

    public static User ToUser(UserViewModel userViewModel)
    {
        return new User
        {
            EmailAddress = userViewModel.EmailAddress,
            FirstName = userViewModel.FirstName,
            LastName = userViewModel.LastName,
            PhoneNumber = userViewModel.PhoneNumber
        };
    }
}