using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Repository.Interfaces;
using TriviaPruebaTecnica.Service.Interfaces;

namespace TriviaPruebaTecnica.Service
{
    public class TriviaService : ITriviaService
    {
        private readonly ITriviaRepository _triviaRepository;

        public TriviaService(ITriviaRepository triviaRepository)
        {
            _triviaRepository = triviaRepository;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _triviaRepository.GetCategoriesAsync();
        }

        public async Task<IEnumerable<Question>> GetGameQuestionsAsync(int categoryId)
        {
            return await _triviaRepository.GetRandomQuestionsByCategoryAsync(categoryId);
        }

        public async Task SaveGameResultAsync(int userId, int categoryId, int pointsEarned, int correctAnswersCount)
        {
            await _triviaRepository.SaveGameResultAsync(userId, categoryId, pointsEarned, correctAnswersCount);
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int topCount = 10)
        {
            return await _triviaRepository.GetLeaderboardAsync(topCount);
        }
    }
}