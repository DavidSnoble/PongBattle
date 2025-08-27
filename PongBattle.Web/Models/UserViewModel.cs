using PongBattle.Domain;
using PongBattle.Utilities;

namespace PongBattle.Web.Models;

public class UserViewModel : IViewModel
{
    public UserViewModel()
    {
    }

    public UserViewModel(User user)
    {
        Id = user.Id;
        EmailAddress = user.EmailAddress;
        FirstName = user.FirstName;
        LastName = user.LastName;
        PhoneNumber = user.PhoneNumber;
    }

    public int? Id { get; set; }

    public string? EmailAddress { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }

    public Dictionary<string, string> Validate()
    {
        var errorMap = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(FirstName))
            errorMap.Add("FirstName", "First Name is Required");
        if (string.IsNullOrWhiteSpace(LastName))
            errorMap.Add("LastName", "Last Name is Required");
        if (!RegexUtilities.IsValidEmail(EmailAddress))
            errorMap.Add("EmailAddress", "EmailAddress is blank or invalid");
        if (string.IsNullOrWhiteSpace(PhoneNumber) || !(PhoneNumber.Length == 10))
            errorMap.Add("PhoneNumber", "PhoneNumber is blank or invalid");
        return errorMap;
    }

    private bool IsValid()
    {
        return FirstName is not null && LastName is not null &&
               EmailAddress is not null;
    }

    public static User ToUser(UserViewModel userViewModel)
    {
        if (!userViewModel.IsValid())
            throw new ArgumentException(
                "UserViewModel is not valid. Required fields (FirstName, LastName, EmailAddress) cannot be null.");

        return new User
        {
            Id = userViewModel.Id,
            EmailAddress = userViewModel.EmailAddress!,
            FirstName = userViewModel.FirstName!,
            LastName = userViewModel.LastName!,
            PhoneNumber = userViewModel.PhoneNumber
        };
    }
}