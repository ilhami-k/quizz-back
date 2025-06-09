using System;
using Core.Models;

namespace Core.UseCases.Abstractions;

public interface IQuizUseCases
{
    Quiz CreateQuiz(Quiz quizToCreate);
    Quiz? GetQuizById(int quizId);
    IEnumerable<Quiz> GetAllQuizzes();
    IEnumerable<Quiz> GetQuizzesByCategoryId(int categoryId);
    QuizResult SubmitQuiz(QuizSubmission submission);
}
