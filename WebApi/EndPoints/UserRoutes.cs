using System;
using Infrastructure.repositories;

namespace Api.EndPoints;

public static class UserRoutes
{
    public static void AddUserRoutes(this WebApplication app)
    {
        app.MapGet("/users", (UserRepository repo) =>
        {
            return repo.GetAllUsers();
        })
        .WithName("GetAllUsers");

        app.MapGet("/users/{id}", (int id, UserRepository repo) =>
        {
            return repo.GetUserById(id);
        })
        .WithName("GetUserById");
    }
}