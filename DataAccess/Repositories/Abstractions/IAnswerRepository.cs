using Infrastructure.Models;
using System.Collections.Generic;

namespace Infrastructure.Repositories.Abstractions
{
    public interface IAnswerRepository
    {
        void AddAnswer(Answer answer); 
        IEnumerable<Answer> GetAllAnswers(); 
        Answer? GetAnswerById(int answerId); 
        IEnumerable<Answer> GetAnswersByQuestionId(int questionId);
    }
}