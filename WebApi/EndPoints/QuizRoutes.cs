using Infrastructure.Repositories.Abstractions;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http; 

namespace Api.EndPoints
{
    public static class QuizRoutes
    {
        public static void AddQuizRoutes(this WebApplication app)
        {
            var quizGroup = app.MapGroup("/quizzes").WithTags("Quizzes");

            quizGroup.MapGet("/", (IQuizRepository repo) =>
            {
                var quizzes = repo.GetAllQuizzes();
                return Results.Ok(quizzes);
            })
            .WithName("GetAllQuizzes")
            .Produces<IEnumerable<Quiz>>(StatusCodes.Status200OK);

            quizGroup.MapGet("/{id}", (int id, IQuizRepository repo) =>
            {
                var quiz = repo.GetQuizById(id);
                if (quiz is null)
                {
                    return Results.NotFound($"Quiz with ID {id} not found.");
                }
                return Results.Ok(quiz);
            })
            .WithName("GetQuizById")
            .Produces<Quiz>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            quizGroup.MapPost("/", (Quiz quiz, IQuizRepository repo) =>
            {
                if (quiz == null || quiz.Creator == null || quiz.category == null)
                {
                    return Results.BadRequest("Quiz, Creator, and Category information must be provided.");
                }
                quiz.CreatedAt = DateTime.UtcNow;
                var createdQuiz = repo.AddQuiz(quiz);
                return Results.CreatedAtRoute("GetQuizById", new { id = createdQuiz.QuizId }, createdQuiz);
            })
            .WithName("CreateQuiz")
            .Produces<Quiz>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            quizGroup.MapDelete("/{id}", (int id, IQuizRepository repo) =>
            {
                var updatedQuiz = repo.ToggleQuizVisibility(id);
                if (updatedQuiz is null)
                {
                    return Results.NotFound($"Quiz with ID {id} not found.");
                }
                return Results.Ok(updatedQuiz);
            })
            .WithName("ToggleQuizVisibility")
            .Produces<Quiz>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}