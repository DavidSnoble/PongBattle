using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PongBattle.Data;
using PongBattle.Domain;
using PongBattle.Web.Models;

namespace PongBattle.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("/users/{userId:int}")]
    public IActionResult UserIndex(int userId)
    {
        var userRepository = new UserRepository();
        var user = userRepository.Get(userId);

        if (user is not null)
        {
            var userViewModel = UserViewModel.FromUser(user);
            return View(userViewModel);
        }
        return View("Views/Home/404.cshtml");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
