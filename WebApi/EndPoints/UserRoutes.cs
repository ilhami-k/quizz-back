using System;
using Infrastructure.Models;
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

        app.MapPut("/user/id", (int id, User updateUser, UserRepository repo) =>
        {
            var existingUser = repo.GetUserById(id);
            if (existingUser == null) return Results.NotFound();

            existingUser.Name = updateUser.Name;
            existingUser.Email = updateUser.Email;

            repo.UpdateUser(existingUser);
            return Results.NoContent();
        })
        .WithName("UpdateUser");

        
    }
}