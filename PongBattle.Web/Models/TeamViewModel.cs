using PongBattle.Domain;

namespace PongBattle.Web.Models;

public class TeamViewModel : IViewModel
{
    public TeamViewModel() { }

    public TeamViewModel(Team team)
    {
        Id = team.Id;
        Name = team.Name;
        PlayerOneId = team.PlayerOneId;
        PlayerTwoId = team.PlayerTwoId;
    }

    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? PlayerOneId { get; set; }
    public int? PlayerTwoId { get; set; }

    // Navigation properties for display
    public User? PlayerOne { get; set; }
    public User? PlayerTwo { get; set; }

    public Dictionary<string, string> Validate()
    {
        var errorMap = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(Name))
            errorMap.Add("Name", "Team Name is Required");

        if (!PlayerOneId.HasValue)
            errorMap.Add("PlayerOneId", "Player One is Required");

        return errorMap;
    }

    private bool IsValid()
    {
        return Name is not null && PlayerOneId.HasValue;
    }

    public static Team ToTeam(TeamViewModel teamViewModel)
    {
        if (!teamViewModel.IsValid())
            throw new ArgumentException(
                "TeamViewModel is not valid. Required fields (Name, PlayerOneId) cannot be null."
            );

        return new Team
        {
            Id = teamViewModel.Id,
            Name = teamViewModel.Name!,
            PlayerOneId = teamViewModel.PlayerOneId!.Value,
            PlayerTwoId = teamViewModel.PlayerTwoId,
        };
    }
}
