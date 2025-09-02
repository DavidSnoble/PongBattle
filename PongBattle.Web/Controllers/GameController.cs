using Microsoft.AspNetCore.Mvc;

namespace PongBattle.Web.Controllers;

public class GameController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}