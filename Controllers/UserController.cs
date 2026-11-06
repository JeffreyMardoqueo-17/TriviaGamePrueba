using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TriviaPruebaTecnica.Service.Interfaces;

namespace TriviaPruebaTecnica.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Trivia");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string loginUsername, string loginPassword)
        {
            // Pasamos los nombres exactos que vienen del form de la vista
            var (success, message, user, totalPoints) = await _userService.AuthenticateAsync(loginUsername, loginPassword);

            TempData["ActiveTab"] = "login";

            if (!success || user == null)
            {
                TempData["LoginError"] = message;
                return RedirectToAction(nameof(Index));
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("TotalPoints", totalPoints.ToString())
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = System.DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            _logger.LogInformation("Usuario {Username} inició sesión correctamente con {Points} puntos.", user.Username, totalPoints);

            return RedirectToAction("Index", "Trivia");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string regUsername, string regPassword)
        {
            var (success, message) = await _userService.RegisterAsync(regUsername, regPassword);

            TempData["ActiveTab"] = "register";

            if (!success)
            {
                TempData["RegisterError"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}