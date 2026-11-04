using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Service.Interfaces;

namespace TriviaPruebaTecnica.Controllers
{
    [Authorize]
    public class TriviaController : Controller
    {
        private readonly ITriviaService _triviaService;

        public TriviaController(ITriviaService triviaService)
        {
            _triviaService = triviaService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _triviaService.GetCategoriesAsync();
            return View(categories);
        }
        public async Task<IActionResult> Play(int categoryId)
        {
            var questions = await _triviaService.GetGameQuestionsAsync(categoryId);
            if (questions == null || !questions.Any())
            {
                TempData["Error"] = "No hay preguntas disponibles para esta categoría.";
                return RedirectToAction(nameof(Index));
            }

            return View(questions);
        }

        // Endpoint POST para guardar el resultado de la partida al terminar
        [HttpPost]
        public async Task<IActionResult> SaveResult([FromBody] GameResultDto dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { success = false, message = "Usuario no autenticado." });

            await _triviaService.SaveGameResultAsync(userId, dto.CategoryId, dto.PointsEarned, dto.CorrectAnswersCount);
            return Json(new { success = true });
        }
        public async Task<IActionResult> Leaderboard()
        {
            var leaderboard = await _triviaService.GetLeaderboardAsync(10);
            return View(leaderboard);
        }
    }
}