using System.Data;
using Dapper;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Infrastructure.Repositories.Abstractions;


namespace Infrastructure.repositories;

public class UserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new ArgumentNullException(nameof(configuration), "Database connection string 'DefaultConnection' not found.");

    private IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

    public IEnumerable<User> GetAllUsers()
    {
        using var connection = CreateConnection();
        return connection.Query<User>(
            "SELECT ID_user AS UserId, username AS Name, email, password_hash AS Password, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes FROM user"
        );
    }

    public User? GetUserById(int userId)
    {
        using var connection = CreateConnection();
        return connection.QuerySingleOrDefault<User>(
            @"SELECT ID_user AS UserId, username AS Name, email, password_hash AS Password, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes 
            FROM user
            WHERE ID_user = @UserId"
        , new {UserId = userId});
    }

    public User? GetUserByUsername(string username)
    {
        using var connection = CreateConnection();
        return connection.QuerySingleOrDefault<User>(
            @"SELECT ID_user AS UserId, username AS Name, email, password_hash AS Password, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes 
            FROM user
            WHERE username = @Username"
        , new {Username = username});
    }

    public void AddUser(User user)
    {
        using var connection = CreateConnection();

        var sql = @"INSERT INTO user (username, email, password_hash, photo_url, is_admin, created_date, created_quizz, taken_quizz) 
        VALUE (@Name, @Email, @Password, @PhotoURL, @IsAdmin, @CreatedAt, @CreatedQuizzes, @ParticipatedQuizzes)";

        connection.Execute(sql, user);
    }

    public void DeleteUser(int userId)
    {
        using var connection = CreateConnection();

        var sql = "DELETE FROM user WHERE ID_user = @UserId";

        connection.Execute(sql, new {UserId = userId});
    }
}
