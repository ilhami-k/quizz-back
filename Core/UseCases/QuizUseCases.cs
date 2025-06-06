using System;
using Core.Models;
using Core.UseCases.Abstractions;
using Core.IGateways;

namespace Core.UseCases;

public class QuizUseCases : IQuizUseCases
{
    private readonly IQuizGateway _quizGateway;
    public QuizUseCases(IQuizGateway quizgateaway)
    {
        _quizGateway = quizgateaway ?? throw new ArgumentNullException(nameof(quizgateaway));
    }
    public Quiz CreateQuiz(Quiz quizToCreate)
    {
        if (quizToCreate == null)
        {
            throw new ArgumentNullException(nameof(quizToCreate));

        }
        if (string.IsNullOrEmpty(quizToCreate.Title))
        {
            throw new ArgumentException("Le titre du quiz ne peut pas etre nul");
        }
        var createdQuiz = _quizGateway.AddQuiz(quizToCreate);
        return createdQuiz;
    }
    public Quiz? GetQuizById(int quizId)
    {
        return _quizGateway.GetQuizById(quizId);
    }
    public IEnumerable<Quiz> GetAllQuizzes()
    {
        return _quizGateway.GetAllQuizzes();
    }

    public IEnumerable<Quiz> GetQuizzesByCategoryId(int categoryId)
    {
        return _quizGateway.GetQuizzesByCategoryId(categoryId);
    }
    
}
