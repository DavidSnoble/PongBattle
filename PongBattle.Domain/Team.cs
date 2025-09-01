namespace PongBattle.Domain;

public class Team : BaseDomainObject
{
    public required string Name { get; set; }
    public required int PlayerOneId { get; set; }
    public int? PlayerTwoId { get; set; }
}
