using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PongBattle.Data;
using PongBattle.Web.Models;
using PongBattle.Web.Utilities;

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
        var userRepository = new UserRepository();
        var teams = teamRepository.GetAll();

        var teamViewModels = teams.Select(t => {
            var viewModel = new TeamViewModel(t);

            // Load PlayerOne if ID exists
            if (viewModel.PlayerOneId.HasValue)
            {
                viewModel.PlayerOne = userRepository.Get(viewModel.PlayerOneId.Value);
            }

            // Load PlayerTwo if ID exists
            if (viewModel.PlayerTwoId.HasValue)
            {
                viewModel.PlayerTwo = userRepository.Get(viewModel.PlayerTwoId.Value);
            }

            return viewModel;
        });

        return View(teamViewModels);
    }

    [Route("/teams/{teamId:int}")]
    public IActionResult DisplayTeam(int teamId)
    {
        var teamRepository = new TeamRepository();
        var userRepository = new UserRepository();
        var team = teamRepository.Get(teamId);

        if (team is not null)
        {
            var teamViewModel = new TeamViewModel(team);

            // Load PlayerOne if ID exists
            if (teamViewModel.PlayerOneId.HasValue)
            {
                teamViewModel.PlayerOne = userRepository.Get(teamViewModel.PlayerOneId.Value);
            }

            // Load PlayerTwo if ID exists
            if (teamViewModel.PlayerTwoId.HasValue)
            {
                teamViewModel.PlayerTwo = userRepository.Get(teamViewModel.PlayerTwoId.Value);
            }

            return View(teamViewModel);
        }

        return View("/Views/Home/404.cshtml");
    }

    [Route("/teams/add")]
    [HttpGet]
    public IActionResult AddTeam()
    {
        var userRepository = new UserRepository();
        var teamViewModel = new TeamViewModel
        {
            Users = userRepository.GetAll()
        };

        return View(teamViewModel);
    }

    [Route("/teams/add")]
    [HttpPost]
    public IActionResult AddTeam(TeamViewModel teamViewModel)
    {
        var userRepository = new UserRepository();
        teamViewModel.Users = userRepository.GetAll();

        return ValidationUtilities.ValidateFormAndRenderView(teamViewModel, ModelState, RedirectToAction("Index"),
            View(teamViewModel),
            () =>
            {
                var teamRepository = new TeamRepository();

                var team = TeamViewModel.ToTeam(teamViewModel);
                teamRepository.Create(team);
            });
    }

    [Route("/teams/{teamId:int}/edit")]
    [HttpGet]
    public IActionResult EditTeam(int teamId)
    {
        var teamRepository = new TeamRepository();
        var userRepository = new UserRepository();
        var team = teamRepository.Get(teamId);

        if (team is null)
        {
            return View("/Views/Home/404.cshtml");
        }

        var teamViewModel = new TeamViewModel(team)
        {
            Users = userRepository.GetAll()
        };

        return View(teamViewModel);
    }

    [Route("/teams/edit")]
    [HttpPost]
    public IActionResult EditTeam(TeamViewModel teamViewModel)
    {
        var userRepository = new UserRepository();
        teamViewModel.Users = userRepository.GetAll();

        return ValidationUtilities.ValidateFormAndRenderView(teamViewModel, ModelState, RedirectToAction("Index"),
            View(teamViewModel),
            () =>
            {
                var teamRepository = new TeamRepository();

                var team = TeamViewModel.ToTeam(teamViewModel);
                teamRepository.Update(team);
            });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
