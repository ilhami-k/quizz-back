using System;

namespace Core.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhotoURL { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public int CreatedQuizzes { get; set; }
    public int ParticipatedQuizzes { get; set; }
}
