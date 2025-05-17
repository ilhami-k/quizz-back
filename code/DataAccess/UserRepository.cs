using System;
using Dapper;
using DataAccess.Models;
using MySql.Data.MySqlClient;

namespace DataAccess;

public class UserRepository
{
    private readonly string _connectionString;
    public UserRepository()
    {
        _connectionString = "Server=localhost;Database=quizzv2;User=root;Password=admin;";
    }
    public IEnumerable<User> GetAllUsers()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Query<User>(
            "SELECT ID_user AS UserId, username AS Name, email, password_hash AS Password, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes FROM user"
        );
    }

    public User? GetUserById(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.QuerySingleOrDefault<User>(
            @"SELECT ID_user AS UserId, username AS Name, email, password_hash AS Password, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes 
            FROM user
            WHERE ID_user = @UserId"
        , new {UserId = userId});
    }

    public void AddUser(User user)
    {
        using var connection = new MySqlConnection(_connectionString);

        var sql = @"INSERT INTO user (username, email, password_hash, photo_url, is_admin, created_date, created_quizz, taken_quizz) 
        VALUE (@Name, @Email, @Password, @PhotoURL, @IsAdmin, @CreatedAt, @CreatedQuizzes, @ParticipatedQuizzes)";

        connection.Execute(sql, user);
    }

    public void DeleteUser(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);

        var sql = "DELETE FROM user WHERE ID_user = @UserId";

        connection.Execute(sql, new {UserId = userId});
    }
}
