using System;

namespace Core.Models;

public class Answer
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime CreatedAt { get; set;}
    
}
