using System;

namespace DataAccess.Models;

public class Answer
{
    public int AnswerId { get; set; }
    public required string AnswerText { get; set; }
    public bool CorrectAnswer { get; set; }
    public DateTime CreatedAt{ get; set;}
}
