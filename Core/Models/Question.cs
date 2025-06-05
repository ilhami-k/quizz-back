using System;
using System.Dynamic;
using System.Formats.Asn1;

namespace Core.Models;

public class Question
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int QuestionType { get; set; }
    public List<Answer>? Answers { get; set; }
    public int Timer { get; set; } = 15;
    public DateTime CreatedAt { get; set; }
    
}
