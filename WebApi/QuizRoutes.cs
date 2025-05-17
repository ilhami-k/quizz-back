using DataAccess;

namespace WebApi;

public static class QuizRoutes
{
    public static void AddQuizRoutes(this WebApplication app)
    {
        app.MapGet("/quizzes", (QuizRepository repo) =>
        {
            return repo.GetAllQuizzes();
        })
        .WithName("GetAllQuizzes");

        app.MapGet("/quizzes/{id}", (int id, QuizRepository repo) =>
        {
            return repo.GetQuizById(id);
        })
        .WithName("GetQuizById");
    }
}
