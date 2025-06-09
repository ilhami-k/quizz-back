using System;
using Core.IGateway;
using Core.IGateways;
using Core.Models;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Utilities.IO;

namespace Infrastructure.Gateways;

public class QuizGateway : IQuizGateway
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;
    public QuizGateway(IQuizRepository quizRepository, IQuestionRepository questionRepository, IAnswerRepository answerRepository)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
    }
    public Core.Models.Quiz AddQuiz(Core.Models.Quiz coreQuiz)
    {
        if (coreQuiz == null)
        {
            throw new ArgumentNullException(nameof(coreQuiz));
        }

        var infraQuiz = new Infrastructure.Models.Quiz
        {
            Title = coreQuiz.Title,
            Description = coreQuiz.Description,
            Dificulty = coreQuiz.Dificulty,
            CategoryId = coreQuiz.CategoryId,
            UserId = coreQuiz.UserId,
            IsVisible = coreQuiz.IsVisible,
            ParticipantsCount = 0,
            TotalQuestions = coreQuiz.Questions?.Count ?? 0,
            Category = new Infrastructure.Models.Category { CategoryId = coreQuiz.CategoryId },
            Creator = new Infrastructure.Models.User { UserId = coreQuiz.UserId },
            CreatedAt = coreQuiz.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : coreQuiz.CreatedAt
        };

        var createdInfraQuiz = _quizRepository.AddQuiz(infraQuiz);

        if (coreQuiz.Questions != null && coreQuiz.Questions.Any())
        {
            var createdCoreQuestions = new List<Core.Models.Question>();
            foreach (var questionItem in coreQuiz.Questions)
            {
                var infraQuestionToSave = new Infrastructure.Models.Question
                {
                    QuestionId = questionItem.QuestionId,
                    QuizId = createdInfraQuiz.QuizId,
                    QuestionText = questionItem.QuestionText,
                    QuestionType = questionItem.QuestionType,
                    Timer = questionItem.Timer,
                    CreatedAt = questionItem.CreatedAt,
                    Answers = questionItem.Answers?.Select(answer => new Infrastructure.Models.Answer
                    {
                        AnswerId = answer.AnswerId,
                        AnswerText = answer.AnswerText,
                        CorrectAnswer = answer.IsCorrect,
                        CreatedAt = answer.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : answer.CreatedAt
                    }).ToList() ?? new List<Infrastructure.Models.Answer>()
                };

                var createdInfraQuestion = _questionRepository.AddQuestion(infraQuestionToSave);

                var createdCoreQuestion = new Core.Models.Question
                {
                    QuestionId = createdInfraQuestion.QuestionId,
                    QuestionText = createdInfraQuestion.QuestionText,
                    QuestionType = createdInfraQuestion.QuestionType,
                    Timer = createdInfraQuestion.Timer,
                    CreatedAt = createdInfraQuestion.CreatedAt,
                    Answers = createdInfraQuestion.Answers?.Select(infAnswer => new Core.Models.Answer //infAnswer == infrastructureAnswer
                    {
                        AnswerId = infAnswer.AnswerId,
                        AnswerText = infAnswer.AnswerText,
                        IsCorrect = infAnswer.CorrectAnswer,
                        CreatedAt = infAnswer.CreatedAt
                    }).ToList()
                };
                createdCoreQuestions.Add(createdCoreQuestion);
            }
            coreQuiz.Questions = createdCoreQuestions;
        }

        coreQuiz.QuizId = createdInfraQuiz.QuizId;
        return coreQuiz;
    }

    public Core.Models.Quiz? GetQuizById(int quizId)
    {
        var infraQuiz = _quizRepository.GetQuizById(quizId);
        if (infraQuiz == null) return null;
        return new Core.Models.Quiz
        {
            QuizId = infraQuiz.QuizId,
            Title = infraQuiz.Title,
            Description = infraQuiz.Description,
            CategoryId = infraQuiz.Category.CategoryId,
            UserId = infraQuiz.Creator.UserId,
            CreatorUsername = infraQuiz.Creator?.Username,
            Dificulty = infraQuiz.Dificulty,
            ParticipantsCount = infraQuiz.ParticipantsCount,
            IsVisible = infraQuiz.IsVisible,
            Questions = infraQuiz.Questions?.Select(infquest => new Core.Models.Question
            {
                QuestionId = infquest.QuestionId,
                QuestionText = infquest.QuestionText,
                QuestionType = infquest.QuestionType,
                Timer = infquest.Timer,
                CreatedAt = infquest.CreatedAt,
                Answers = infquest.Answers?.Select(infAnswer => new Core.Models.Answer
                {
                    AnswerId = infAnswer.AnswerId,
                    AnswerText = infAnswer.AnswerText,
                    IsCorrect = infAnswer.CorrectAnswer,
                    CreatedAt = infAnswer.CreatedAt
                }).ToList()
            }).ToList()

        };


    }
    public IEnumerable<Core.Models.Quiz> GetAllQuizzes()
    {
        var infraQuizzes = _quizRepository.GetAllQuizzes();

        return infraQuizzes.Select(infraQuiz => new Core.Models.Quiz
        {
            QuizId = infraQuiz.QuizId,
            Title = infraQuiz.Title,
            Description = infraQuiz.Description,
            Dificulty = infraQuiz.Dificulty,
            CategoryId = infraQuiz.Category?.CategoryId ?? 0,
            UserId = infraQuiz.Creator?.UserId ?? 0,
            CreatorUsername = infraQuiz.Creator?.Username,
            ParticipantsCount = infraQuiz.ParticipantsCount,
            CreatedAt = infraQuiz.CreatedAt,
            IsVisible = infraQuiz.IsVisible,
            Questions = infraQuiz.Questions?.Select(iq => new Core.Models.Question
            {
                QuestionId = iq.QuestionId,
                QuestionText = iq.QuestionText,
                QuestionType = iq.QuestionType,
                Timer = iq.Timer,
                CreatedAt = iq.CreatedAt,
                Answers = iq.Answers?.Select(ia => new Core.Models.Answer
                {
                    AnswerId = ia.AnswerId,
                    AnswerText = ia.AnswerText,
                    IsCorrect = ia.CorrectAnswer,
                    CreatedAt = ia.CreatedAt
                }).ToList()
            }).ToList()
        }).ToList();
    }
    public Core.Models.Question AddQuestionToQuiz(int quizId, Core.Models.Question coreQuestion)
    {
        if (coreQuestion == null)
            throw new ArgumentNullException(nameof(coreQuestion));

        var infraQuestionToSave = new Infrastructure.Models.Question
        {
            QuestionId = coreQuestion.QuestionId,
            QuizId = quizId,
            QuestionText = coreQuestion.QuestionText,
            QuestionType = coreQuestion.QuestionType,
            Timer = coreQuestion.Timer,
            CreatedAt = coreQuestion.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : coreQuestion.CreatedAt,
            Answers = coreQuestion.Answers?.Select(cqAnswer => new Infrastructure.Models.Answer
            {
                AnswerId = cqAnswer.AnswerId,
                AnswerText = cqAnswer.AnswerText,
                CorrectAnswer = cqAnswer.IsCorrect,
                CreatedAt = cqAnswer.CreatedAt
            }).ToList() ?? new List<Infrastructure.Models.Answer>()
        };

        var createdInfraQuestion = _questionRepository.AddQuestion(infraQuestionToSave);

        return new Core.Models.Question
        {
            QuestionId = createdInfraQuestion.QuestionId,
            QuestionText = createdInfraQuestion.QuestionText,
            QuestionType = createdInfraQuestion.QuestionType,
            Timer = createdInfraQuestion.Timer,
            CreatedAt = createdInfraQuestion.CreatedAt,
            Answers = createdInfraQuestion.Answers?.Select(ia => new Core.Models.Answer
            {
                AnswerId = ia.AnswerId,
                AnswerText = ia.AnswerText,
                IsCorrect = ia.CorrectAnswer,
                CreatedAt = ia.CreatedAt
            }).ToList()
        };
    }
    public Core.Models.Question? GetQuestionById(int questionId)
    {
        var infraQuestion = _questionRepository.GetQuestionById(questionId);
        if (infraQuestion == null) return null;

        return new Core.Models.Question
        {
            QuestionId = infraQuestion.QuestionId,
            QuestionText = infraQuestion.QuestionText,
            QuestionType = infraQuestion.QuestionType,
            Timer = infraQuestion.Timer,
            CreatedAt = infraQuestion.CreatedAt,
            Answers = infraQuestion.Answers?.Select(ia => new Core.Models.Answer
            {
                AnswerId = ia.AnswerId,
                AnswerText = ia.AnswerText,
                IsCorrect = ia.CorrectAnswer,
                CreatedAt = ia.CreatedAt
            }).ToList()
        };
    }
    public IEnumerable<Core.Models.Question> GetQuestionsByQuizId(int quizId)
    {
        var infraQuestions = _questionRepository.GetQuestionsByQuizId(quizId);
        return infraQuestions.Select(infraQuestion => new Core.Models.Question
        {
            QuestionId = infraQuestion.QuestionId,
            QuestionText = infraQuestion.QuestionText,
            QuestionType = infraQuestion.QuestionType,
            Timer = infraQuestion.Timer,
            CreatedAt = infraQuestion.CreatedAt,
            Answers = infraQuestion.Answers?.Select(ia => new Core.Models.Answer
            {
                AnswerId = ia.AnswerId,
                AnswerText = ia.AnswerText,
                IsCorrect = ia.CorrectAnswer,
                CreatedAt = ia.CreatedAt
            }).ToList()
        }).ToList();
    }
    public IEnumerable<Core.Models.Answer> GetAnswersByQuestionId(int questionId)
    {
        var infraQuestion = _questionRepository.GetQuestionById(questionId);
        if (infraQuestion == null || infraQuestion.Answers == null)
            return Enumerable.Empty<Core.Models.Answer>();
        return infraQuestion.Answers.Select(ia => new Core.Models.Answer
        {
            AnswerId = ia.AnswerId,
            AnswerText = ia.AnswerText,
            IsCorrect = ia.CorrectAnswer,
            CreatedAt = ia.CreatedAt
        }).ToList();
    }
    
    public IEnumerable<Core.Models.Quiz> GetQuizzesByCategoryId(int categoryId)
    {
        var infraQuizzes = _quizRepository.GetQuizzesByCategoryId(categoryId);
        return infraQuizzes.Select(infraQuiz => new Core.Models.Quiz
        {
            QuizId = infraQuiz.QuizId,
            Title = infraQuiz.Title,
            Description = infraQuiz.Description,
            Dificulty = infraQuiz.Dificulty,
            CategoryId = infraQuiz.Category?.CategoryId ?? 0,
            UserId = infraQuiz.Creator?.UserId ?? 0,
            CreatorUsername = infraQuiz.Creator?.Username,
            ParticipantsCount = infraQuiz.ParticipantsCount,
            CreatedAt = infraQuiz.CreatedAt,
            IsVisible = infraQuiz.IsVisible,
            Questions = infraQuiz.Questions?.Select(iq => new Core.Models.Question
            {
                QuestionId = iq.QuestionId,
                QuestionText = iq.QuestionText,
                QuestionType = iq.QuestionType,
                Timer = iq.Timer,
                CreatedAt = iq.CreatedAt,
                Answers = iq.Answers?.Select(ia => new Core.Models.Answer
                {
                    AnswerId = ia.AnswerId,
                    AnswerText = ia.AnswerText,
                    IsCorrect = ia.CorrectAnswer,
                    CreatedAt = ia.CreatedAt
                }).ToList()
            }).ToList()
        }).ToList();
    }
        
}
