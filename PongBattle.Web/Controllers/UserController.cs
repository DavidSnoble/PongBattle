using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PongBattle.Data;
using PongBattle.Web.Models;
using PongBattle.Web.Utilities;

namespace PongBattle.Web.Controllers;

public class UserController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public UserController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Route("/users")]
    public IActionResult Index()
    {
        var userRepository = new UserRepository();
        var users = userRepository.GetAll();

        var userViewModels = users.Select(u => new UserViewModel(u));

        return View(userViewModels);
    }

    [Route("/users/{userId:int}")]
    public IActionResult DisplayUser(int userId)
    {
        var userRepository = new UserRepository();
        var user = userRepository.Get(userId);

        if (user is not null)
        {
            var userViewModel = new UserViewModel(user);
            return View(userViewModel);
        }

        return View("/Views/Home/404.cshtml");
    }

    [Route("/users/add")]
    [HttpGet]
    public IActionResult AddUser()
    {
        return View(new UserViewModel());
    }

    [Route("/users/add")]
    [HttpPost]
    public IActionResult AddUser(UserViewModel userViewModel)
    {
        return ValidationUtilities.ValidateFormAndRenderView(userViewModel, ModelState, RedirectToAction("Index"),
            View(userViewModel),
            () =>
            {
                var userRepository = new UserRepository();

                var user = UserViewModel.ToUser(userViewModel);
                userRepository.Create(user);
            });
    }

    [Route("/users/{userId:int}/edit")]
    [HttpGet]
    public IActionResult EditUser(int userId)
    {
        var userRepository = new UserRepository();
        var user = userRepository.Get(userId);

        if (user is null) return View("/Views/Home/404.cshtml");
        var userViewModel = new UserViewModel(user);


        return View(userViewModel);
    }

    [Route("/users/edit")]
    [HttpPost]
    public IActionResult EditUser(UserViewModel userViewModel)
    {
        return ValidationUtilities.ValidateFormAndRenderView(userViewModel, ModelState, RedirectToAction("Index"),
            View(userViewModel),
            () =>
            {
                var userRepository = new UserRepository();

                var user = UserViewModel.ToUser(userViewModel);
                userRepository.Update(user);
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