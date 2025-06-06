using Core.Models;
using Core.UseCases.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.EndPoints
{
    public static class QuizRoutes
    {
        public static void AddQuizRoutes(this WebApplication app)
        {
            var quizGroup = app.MapGroup("/quizzes")
                               .WithTags("Quizzes")
                               .WithOpenApi();

            quizGroup.MapGet("/", (IQuizUseCases quizUseCases) =>
            {
                var quizzes = quizUseCases.GetAllQuizzes();
                return Results.Ok(quizzes);
            })
            .WithName("GetAllQuizzes")
            .Produces<IEnumerable<Core.Models.Quiz>>(StatusCodes.Status200OK);

            quizGroup.MapGet("/{id:int}", (int id, IQuizUseCases quizUseCases) =>
            {
                var quiz = quizUseCases.GetQuizById(id);
                if (quiz is null)
                {
                    return Results.NotFound($"Quiz with ID {id} not found.");
                }
                return Results.Ok(quiz);
            })
            .WithName("GetQuizById")
            .Produces<Core.Models.Quiz>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            quizGroup.MapPost("/create", ([FromBody] Core.Models.Quiz quizToCreate, IQuizUseCases quizUseCases) =>
            {
                try
                {
                    if (quizToCreate == null)
                    {
                        return Results.BadRequest("Invalid quiz data provided.");
                    }
                    var createdQuiz = quizUseCases.CreateQuiz(quizToCreate);
                    return Results.CreatedAtRoute("GetQuizById", new { id = createdQuiz.QuizId }, createdQuiz);
                }
                catch (ArgumentException argEx)
                {
                    return Results.BadRequest(new { message = argEx.Message });
                }
                catch (Exception)
                {
                    return Results.Problem("An unexpected error occurred while creating the quiz.", statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("CreateQuiz")
            .Produces<Core.Models.Quiz>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            quizGroup.MapGet("/category/{categoryId:int}", (int categoryId, IQuizUseCases quizUseCases) =>
            {
                var quizzes = quizUseCases.GetQuizzesByCategoryId(categoryId);
                if (quizzes is null || !quizzes.Any())
                {
                    return Results.NotFound($"No quizzes found for category ID {categoryId}.");
                }
                return Results.Ok(quizzes);
            })
            .WithName("GetQuizzesByCategoryId");
        } 
    }
}