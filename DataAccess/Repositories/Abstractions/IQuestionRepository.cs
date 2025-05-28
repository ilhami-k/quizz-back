using Infrastructure.Models;
using System.Collections.Generic;

namespace Infrastructure.Repositories.Abstractions
{
    public interface IQuestionRepository
    {
        IEnumerable<Question> GetQuestionsByQuizId(int quizId);
        Question? GetQuestionById(int questionId);
        Question AddQuestion(Question question); 
    }
}