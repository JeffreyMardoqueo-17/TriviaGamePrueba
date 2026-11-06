using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Repository.Interfaces;
using TriviaPruebaTecnica.Service.Interfaces;

namespace TriviaPruebaTecnica.Service
{
    public class TriviaService : ITriviaService
    {
        private readonly ITriviaRepository _triviaRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TriviaService> _logger;

        private const string CategoriesCacheKey = "Categories_Cache_Key";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public TriviaService(
            ITriviaRepository triviaRepository, 
            IMemoryCache memoryCache,
            ILogger<TriviaService> logger)
        {
            _triviaRepository = triviaRepository ?? throw new ArgumentNullException(nameof(triviaRepository));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categories = await _memoryCache.GetOrCreateAsync(CategoriesCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                _logger.LogInformation("Caché de categorías expirado. Consultando base de datos...");
                
                var result = await _triviaRepository.GetCategoriesAsync();
                return result ?? Enumerable.Empty<Category>();
            });

            return categories ?? Enumerable.Empty<Category>();
        }

        public async Task<IEnumerable<Question>> GetGameQuestionsAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                _logger.LogWarning("Intento de obtener preguntas con CategoryId inválido: {CategoryId}", categoryId);
                throw new ArgumentException("El identificador de categoría no es válido.", nameof(categoryId));
            }

            try
            {
                var questions = await _triviaRepository.GetRandomQuestionsByCategoryAsync(categoryId);
                return questions ?? Enumerable.Empty<Question>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar preguntas para la categoría {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task SaveGameResultAsync(int userId, int categoryId, int pointsEarned, int correctAnswersCount)
        {
            if (userId <= 0)
                throw new ArgumentException("ID de usuario inválido.", nameof(userId));
            
            if (categoryId <= 0)
                throw new ArgumentException("ID de categoría inválido.", nameof(categoryId));

            if (pointsEarned < 0 || correctAnswersCount < 0)
                throw new ArgumentException("Los puntos o conteo de aciertos no pueden ser negativos.");

            try
            {
                await _triviaRepository.SaveGameResultAsync(userId, categoryId, pointsEarned, correctAnswersCount);
                _logger.LogInformation("Resultado guardado: Usuario {UserId}, Categoría {CategoryId}, Puntos {Points}", userId, categoryId, pointsEarned);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el resultado de la partida para el usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int topCount = 10)
        {
            // Regla de negocio: Acotar el top para proteger recursos
            if (topCount <= 0 || topCount > 100)
            {
                topCount = 10;
            }

            var leaderboard = await _triviaRepository.GetLeaderboardAsync(topCount);
            return leaderboard ?? Enumerable.Empty<LeaderboardEntry>();
        }
    }
}