using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Repository.Interfaces;

namespace TriviaPruebaTecnica.Repository
{
    public class TriviaRepository : ITriviaRepository
    {
        private readonly IDbConnection _dbConnection;

        public TriviaRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _dbConnection.QueryAsync<Category>(
                "sp_GetCategories", 
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Question>> GetRandomQuestionsByCategoryAsync(int categoryId)
        {
            using var multi = await _dbConnection.QueryMultipleAsync(
                "sp_GetRandomQuestionsByCategory",
                new { CategoryId = categoryId },
                commandType: CommandType.StoredProcedure
            );

            var questions = (await multi.ReadAsync<Question>()).ToList();
            var answers = (await multi.ReadAsync<Answer>()).ToList();
            foreach (var q in questions)
                q.Answers = answers.Where(a => a.QuestionId == q.QuestionId).ToList();

            return questions;
        }

        public async Task SaveGameResultAsync(int userId, int categoryId, int pointsEarned, int correctAnswersCount)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@CategoryId", categoryId);
            parameters.Add("@PointsEarned", pointsEarned);
            parameters.Add("@CorrectAnswersCount", correctAnswersCount);

            await _dbConnection.ExecuteAsync(
                "sp_SaveGameResult",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int topCount = 10)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TopCount", topCount);

            return await _dbConnection.QueryAsync<LeaderboardEntry>(
                "sp_GetLeaderboard",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}