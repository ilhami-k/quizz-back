using System;
namespace DataAccess.Models;

public class Quiz
{
    public int QuizId {get; set;}
    public int UserId {get; set;}
    public int CategoryId {get; set;}
    public string Title {get;set;} = string.Empty;
    public string? Description {get; set;}
    public int Dificulty {get; set;}
    public DateTime CreatedAt { get; set; }
    public int ParticipantsCount {get; set;}
    public int TotalQuestions {get; set;}
    public bool IsVisible {get; set;}
    public required User Creator { get; set; }
    public required Category category { get; set; }
}
