using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PongBattle.Data;
using PongBattle.Web.Models;

namespace PongBattle.Web.Controllers;

public class TeamController : Controller
{
    private readonly ILogger<TeamController> _logger;

    public TeamController(ILogger<TeamController> logger)
    {
        _logger = logger;
    }

    [Route("/teams")]
    public IActionResult Index()
    {
        var teamRepository = new TeamRepository();
        var teams = teamRepository.GetAll();

        var teamViewModels = teams.Select(t => new TeamViewModel(t));

        return View(teamViewModels);
    }

    [Route("/teams/{teamId:int}")]
    public IActionResult DisplayTeam(int teamId)
    {
        var teamRepository = new TeamRepository();
        var team = teamRepository.Get(teamId);

        if (team is not null)
        {
            var teamViewModel = new TeamViewModel(team);
            return View(teamViewModel);
        }

        return View("/Views/Home/404.cshtml");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
