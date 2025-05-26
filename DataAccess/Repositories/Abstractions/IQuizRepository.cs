using Infrastructure.Models;
using System.Collections.Generic;

namespace Infrastructure.Repositories.Abstractions
{
    public interface IQuizRepository
    {
        Quiz? GetQuizById(int quizId);
        IEnumerable<Quiz> GetAllQuizzes();
        Quiz AddQuiz(Quiz quiz); 
        Quiz? ToggleQuizVisibility(int quizId);
    }
}