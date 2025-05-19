using System;
using System.Formats.Asn1;

namespace DataAccess.Models;

public class Question
{
    public int QuestionId { get; set; }
    public int QuizId { get; set; }
    public required string QuestionText { get; set; }
    public int QuestionType { get; set; }
    public int Timer { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<Answer>? Answers{ get; set;}
}
