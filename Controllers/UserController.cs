using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        // GET: /User o /User/Index (Muestra la vista unificada de Login y Registro)
        [HttpGet]
        public IActionResult Index()
        {
            // Si el usuario ya está autenticado, lo mandamos directo al juego
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Trivia");
            }

            return View();
        }

        // POST: /User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string regUsername, string regPassword)
        {
            var (success, message) = await _userService.RegisterAsync(regUsername, regPassword);

            if (!success)
            {
                TempData["RegisterError"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "¡Registro exitoso! Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string loginUsername, string loginPassword)
        {
            var (success, message, user) = await _userService.AuthenticateAsync(loginUsername, loginPassword);

            if (!success || user == null)
            {
                TempData["LoginError"] = message;
                return RedirectToAction(nameof(Index));
            }

            // Crear los Claims de identidad para la Cookie segura
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Emitir la Cookie cifrada HttpOnly
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _logger.LogInformation("Usuario {Username} ha iniciado sesión exitosamente.", user.Username);

            // Redirigir al controlador de Trivia (el juego)
            return RedirectToAction("Index", "Trivia");
        }

        // POST: /User/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}