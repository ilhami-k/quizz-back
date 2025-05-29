using System;
using Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.UseCases.Abstractions;
using MySqlX.XDevAPI.Common;

namespace Api.EndPoints;

public static class UserRoutes
{
    public static void AddUserRoutes(this WebApplication app)
    {
        app.MapGet("/users", (IUserRepository repo) =>
        {
            return repo.GetAllUsers();
        })
        .WithName("GetAllUsers");

        app.MapGet("/users/{id:int}", (int id, IUserRepository repo) =>
        {
            return repo.GetUserById(id);
        })
        .WithName("GetUserById");

        app.MapGet("/users/{username}", (string username, IUserRepository repo) =>
        {
            return repo.GetUserByUsername(username);
        })
        .WithName("GetUserByUsername");

        app.MapPost("/users/register", ([FromBody] RegisterRequest request, IUserUseCases useCases) =>
        {
            useCases.Register(request);
            return Results.Created();
        })
        .WithName("Register");

        // Ajouter le Jwt token pour la sécurité !
        app.MapPost("/users/auth", ([FromBody] AuthenticationRequest request, IUserUseCases useCases) =>
        {
            var user = useCases.AuthenticateAndGetUser(request);
            if (user == null)
            {
                return Results.Unauthorized();
            }
            return Results.Ok(user);
        });
    }
}