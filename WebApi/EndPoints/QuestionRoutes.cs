using Infrastructure.Models;
using Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.EndPoints;

public static class QuestionRoutes
{
    public static void AddQuestionRoutes(this WebApplication app)
    {
        var questionGroup = app.MapGroup("/quizzes/{quizId}/questions").WithTags("Questions");
        questionGroup.MapGet("/", (int quizId, IQuestionRepository repo) =>
        {
            var questions = repo.GetQuestionsByQuizId(quizId);

            return Results.Ok(questions);
        })
        .WithName("GetQuestionsByQuizId")
        .Produces<IEnumerable<Question>>(StatusCodes.Status200OK); 

        questionGroup.MapGet("/{questionId}", (int quizId, int questionId, IQuestionRepository repo) =>
        {
            var question = repo.GetQuestionById(questionId); 
            if (question == null || question.QuizId != quizId) 
            {
                return Results.NotFound($"Question with ID {questionId} not found in quiz {quizId}.");
            }
            return Results.Ok(question);
        })
        .WithName("GetQuestionById")
        .Produces<Question>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        questionGroup.MapPost("/", (int quizId, [FromBody] Question question, IQuestionRepository questionRepo, IQuizRepository quizRepo) =>
        {
            var quizExists = quizRepo.GetQuizById(quizId); 
            if (quizExists == null)
            {
                return Results.NotFound($"Quiz with ID {quizId} not found. Cannot add question.");
            }

            if (question == null || string.IsNullOrWhiteSpace(question.QuestionText))
            {
                return Results.BadRequest("Question data is required and must include QuestionText.");
            }

            question.QuizId = quizId; 
            question.CreatedAt = DateTime.UtcNow;

            if (question.Answers != null)
            {
                foreach (var answer in question.Answers)
                {
                    answer.CreatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                question.Answers = new List<Answer>(); 
            }
            
            var createdQuestion = questionRepo.AddQuestion(question); 
            return Results.CreatedAtRoute("GetQuestionById", new { quizId = createdQuestion.QuizId, questionId = createdQuestion.QuestionId }, createdQuestion);
        })
        .WithName("CreateQuestion")
        .Produces<Question>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}