using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TriviaPruebaTecnica.Models;
using TriviaPruebaTecnica.Repository.Interfaces;

namespace TriviaPruebaTecnica.Repository
{
    public class TriviaRepository : ITriviaRepository
    {
        private readonly string _connectionString;

        public TriviaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Category>(
                "sp_GetCategories", 
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Question>> GetRandomQuestionsByCategoryAsync(int categoryId)
        {
            using var connection = new SqlConnection(_connectionString);

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetRandomQuestionsByCategory",
                new { CategoryId = categoryId },
                commandType: CommandType.StoredProcedure
            );

            var questions = (await multi.ReadAsync<Question>()).ToList();
            var answers = (await multi.ReadAsync<Answer>()).ToList();

            foreach (var q in questions)
            {
                q.Answers = answers.Where(a => a.QuestionId == q.QuestionId).ToList();
            }

            return questions;
        }
        public async Task SaveGameResultAsync(int userId, int categoryId, int pointsEarned, int correctAnswersCount)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "sp_SaveGameResult",
                new { UserId = userId, CategoryId = categoryId, PointsEarned = pointsEarned, CorrectAnswersCount = correctAnswersCount },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int topCount = 10)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<LeaderboardEntry>(
                "sp_GetLeaderboard",
                new { TopCount = topCount },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}