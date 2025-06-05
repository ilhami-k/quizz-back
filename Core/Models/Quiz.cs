using System;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;

namespace Core.Models;

public class Quiz
{
    public required int QuizId { get; set; }
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public int CategoryId { get; set; }

    public int Dificulty { get; set; }
    public int UserId { get; set; }
    public string? CreatorUsername{ get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVisible { get; set; } = true;
    public List<Question>? Questions{ get; set; }
    

}
