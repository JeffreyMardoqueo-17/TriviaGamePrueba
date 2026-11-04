using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TriviaPruebaTecnica.Models;

namespace TriviaPruebaTecnica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Si entran a la raíz '/', puedes redirigirlos directamente al Login de User o a Trivia
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Trivia");
            }
            return RedirectToAction("Index", "User");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}