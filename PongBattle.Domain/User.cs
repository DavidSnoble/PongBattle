namespace PongBattle.Domain;

public class User : BaseDomainObject
{
    public required string EmailAddress { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? PhoneNumber { get; set; }
}