using System;
using Infrastructure.Models;
using Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.UseCases.Abstractions;
using Microsoft.IdentityModel.Tokens;


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

        app.MapPut("/users/{id:int}", (int id, Core.Models.User updateUser, UserRepository repo) =>
        {
            var existingUser = repo.GetUserById(id);
            if (existingUser == null) return Results.NotFound();

            existingUser.Username = updateUser.Username;
            existingUser.Email = updateUser.Email;

            repo.UpdateUser(existingUser);
            return Results.NoContent();
        })
        .WithName("UpdateUser");

     
        app.MapPost("/users/register", ([FromBody] RegisterRequest request, IUserUseCases useCases) =>
        {
            useCases.Register(request);
            return Results.Created();
        })
        .WithName("Register");

        // Ajouter le Jwt token pour la sécurité !
        app.MapPost("/users/auth", ([FromBody] AuthenticationRequest request, IUserUseCases useCases, IConfiguration configuration) =>
        {
            var user = useCases.AuthenticateAndGetUser(request);
            if (user != null)
            {
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
                var expireTime = configuration["Jwt:ExpireTimeInMinutes"];
                var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expireTime ?? "5"));

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                };

                if (user.IsAdmin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expiration,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                return Results.Ok(new { authToken = jwtToken, user });
            }
            else
            {
                return Results.Unauthorized();
            }
        })
        .WithName("Authentication");

        app.MapDelete("/users/delete/{id}", (int id, IUserRepository repo) =>
        {
            var user = repo.GetUserById(id);
            if (user == null)
            {
                return Results.NotFound($"User with ID {id} not found.");
            }

            repo.DeleteUser(id);
            return Results.NoContent();
        })
        .WithName("DeleteUser");
    }
}