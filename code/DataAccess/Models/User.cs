using System;
namespace DataAccess.Models;

public class User
{
    public int UserId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Password {get;set;} = string.Empty;
    public string PhotoURL{get;set;} = string.Empty;
    public int IsAdmin {get;set;}
    public DateTime CreatedAt{get;set;}
    public int CreatedQuizzes { get; set; }
    public int ParticipatedQuizzes { get; set; }
}
