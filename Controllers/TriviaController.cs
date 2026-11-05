using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Service.Interfaces;

namespace TriviaPruebaTecnica.Controllers
{
    [Authorize] 
    public class TriviaController : Controller
    {
        private readonly ITriviaService _triviaService;
        private readonly ILogger<TriviaController> _logger;

        public TriviaController(ITriviaService triviaService, ILogger<TriviaController> logger)
        {
            _triviaService = triviaService ?? throw new ArgumentNullException(nameof(triviaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _triviaService.GetCategoriesAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías en la vista principal.");
                TempData["Error"] = "Ocurrió un error al cargar las categorías de la trivia.";
                return View(Enumerable.Empty<Category>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Play(int categoryId)
        {
            if (categoryId <= 0)
            {
                _logger.LogWarning("Intento de jugar con un CategoryId inválido: {CategoryId}", categoryId);
                TempData["Error"] = "Categoría seleccionada no válida.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var questions = await _triviaService.GetGameQuestionsAsync(categoryId);
                
                if (questions == null || !questions.Any())
                {
                    TempData["Error"] = "No hay preguntas disponibles para esta categoría.";
                    return RedirectToAction(nameof(Index));
                }

                return View(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar las preguntas para la categoría {CategoryId}", categoryId);
                TempData["Error"] = "Ocurrió un error al preparar la partida.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveResult([FromBody] GameResultDto? dto)
        {
            if (dto == null)
            {
                return BadRequest(new { success = false, message = "Datos de resultado inválidos." });
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Intento de guardar resultado de trivia sin una sesión de usuario válida.");
                return Unauthorized(new { success = false, message = "Usuario no autenticado o sesión expirada." });
            }

            try
            {
                await _triviaService.SaveGameResultAsync(userId, dto.CategoryId, dto.PointsEarned, dto.CorrectAnswersCount);
                
                _logger.LogInformation("Resultado de trivia guardado exitosamente para el usuario ID: {UserId}", userId);
                return Json(new { success = true, message = "Resultado guardado correctamente." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Datos de juego inválidos enviados por el usuario ID: {UserId}", userId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al guardar el resultado de la partida para el usuario ID: {UserId}", userId);
                return StatusCode(500, new { success = false, message = "Ocurrió un error interno al procesar el resultado." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Leaderboard()
        {
            try
            {
                var leaderboard = await _triviaService.GetLeaderboardAsync(10);
                return View(leaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar la tabla de clasificación.");
                TempData["Error"] = "No se pudo cargar la tabla de líderes.";
                return View(Enumerable.Empty<LeaderboardEntry>());
            }
        }
    }
}