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
        public QuizResult SubmitQuiz(QuizSubmission submission)
    {
        if (submission == null || submission.Answers == null)
        {
            throw new ArgumentNullException(nameof(submission), "La soumission ne peut pas être nulle.");
        }

        var quiz = _quizGateway.GetQuizById(submission.QuizId);
        if (quiz == null || quiz.Questions == null)
        {
            throw new KeyNotFoundException($"Le quiz avec l'ID {submission.QuizId} n'a pas été trouvé ou ne contient pas de questions.");
        }

        int score = 0;
        foreach (var userAnswer in submission.Answers)
        {
            var question = quiz.Questions.FirstOrDefault(q => q.QuestionId == userAnswer.QuestionId);
            if (question != null && question.Answers != null)
            {
                var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
                if (correctAnswer != null && correctAnswer.AnswerId == userAnswer.AnswerId)
                {
                    score++;
                }
            }
        }

        return new QuizResult
        {
            Score = score,
            TotalQuestions = quiz.Questions.Count,
            Message = $"Votre score est de {score} sur {quiz.Questions.Count}."
        };
    }
    
}
