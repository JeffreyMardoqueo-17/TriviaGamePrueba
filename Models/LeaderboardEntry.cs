using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaPruebaTecnica.Models
{
    public class LeaderboardEntry
    {
        public long RankPosition { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int GamesPlayed { get; set; }
        public int CorrectAnswers { get; set; }
    }

    public class GameResultDto
    {
        public int CategoryId { get; set; }
        public int PointsEarned { get; set; }
        public int CorrectAnswersCount { get; set; }
    }
}