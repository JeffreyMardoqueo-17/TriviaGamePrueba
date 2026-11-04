using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaPruebaTecnica.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int CategoryId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<Answer> Answers { get; set; } = new();
    }
}