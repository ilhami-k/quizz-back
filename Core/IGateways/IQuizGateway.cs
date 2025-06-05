using System;
using Core.Models;

namespace Core.IGateways;

public interface IQuizGateway
{
    Quiz? GetQuizById(int quizId);
    IEnumerable<Quiz> GetAllQuizzes();
    Quiz AddQuiz(Quiz quiz);
    IEnumerable<Question> GetQuestionsByQuizId(int quizId);
    Question? GetQuestionById(int questionId);
    Question AddQuestionToQuiz(int quizId, Question question);
    IEnumerable<Answer> GetAnswersByQuestionId(int questionId);




}
