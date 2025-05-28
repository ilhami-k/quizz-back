using Infrastructure.Models;
using Infrastructure.repositories; // Assuming QuestionRepository will be in this namespace
using Infrastructure.Repositories.Abstractions; // Assuming IQuestionRepository will be in this namespace
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; // Required for [FromBody]

namespace Api.EndPoints;

public static class QuestionRoutes
{
    public static void AddQuestionRoutes(this WebApplication app)
    {
        var questionGroup = app.MapGroup("/quizzes/{quizId}/questions").WithTags("Questions");

        // GET all questions for a specific quiz
        questionGroup.MapGet("/", (int quizId, IQuestionRepository repo) =>
        {
            var questions = repo.GetQuestionsByQuizId(quizId); // Synchronous call
            if (questions == null || !questions.Any())
            {
                // Optional: Could return NotFound if the quiz itself doesn't exist,
                // or if it exists but has no questions. For now, Ok with empty list is fine.
            }
            return Results.Ok(questions);
        })
        .WithName("GetQuestionsByQuizId")
        .Produces<IEnumerable<Question>>(StatusCodes.Status200OK); // Removed 404 for now, depends on desired behavior for no questions

        // GET a specific question by its ID within a quiz
        questionGroup.MapGet("/{questionId}", (int quizId, int questionId, IQuestionRepository repo) =>
        {
            var question = repo.GetQuestionById(questionId); // Synchronous call
            if (question == null || question.QuizId != quizId) 
            {
                return Results.NotFound($"Question with ID {questionId} not found in quiz {quizId}.");
            }
            return Results.Ok(question);
        })
        .WithName("GetQuestionById")
        .Produces<Question>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST a new question to a specific quiz using the domain model directly
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