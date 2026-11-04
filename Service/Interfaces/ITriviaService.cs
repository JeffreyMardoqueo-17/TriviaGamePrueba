using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaPruebaTecnica.Models;

namespace TriviaPruebaTecnica.Service.Interfaces
{
    public interface ITriviaService
    {
       Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<Question>> GetGameQuestionsAsync(int categoryId);
        Task SaveGameResultAsync(int userId, int categoryId, int pointsEarned, int correctAnswersCount);
        Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int topCount = 10);
    }
}