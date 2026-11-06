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
        public async Task<IActionResult> Register(string regUsername, string regPassword)
        {
            // Llamamos al servicio que valida si el usuario existe
            var result = await _userService.RegisterAsync(regUsername, regPassword);

            if (!result.Success)
            {
                // Guardamos el mensaje exacto que viene del servicio ("El nombre de usuario ya está registrado...")
                TempData["RegisterError"] = result.Message;

                // Marcamos la pestaña activa para que la vista abra automáticamente el formulario de registro
                TempData["ActiveTab"] = "register";

                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = result.Message; // "Usuario registrado exitosamente."
            TempData["ActiveTab"] = "login";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string loginUsername, string loginPassword)
        {
            // Recibimos los 4 valores devueltos por el servicio actualizado
            var (success, message, user, totalPoints) = await _userService.AuthenticateAsync(loginUsername, loginPassword);

            if (!success || user == null)
            {
                TempData["LoginError"] = message;
                return RedirectToAction(nameof(Index));
            }

            // Creamos las claims incluyendo los puntos totales para mostrarlos en el Navbar
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("TotalPoints", totalPoints.ToString()) // <-- Claim indispensable para el Navbar
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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}