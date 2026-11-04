using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaPruebaTecnica.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}