using System.Data;
using Dapper;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Infrastructure.Repositories.Abstractions;


namespace Infrastructure.Repositories;

public class UserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new ArgumentNullException(nameof(configuration), "Database connection string 'DefaultConnection' not found.");

    private IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

    public IEnumerable<User> GetAllUsers()
    {
        using var connection = CreateConnection();
        return connection.Query<User>(
            "SELECT ID_user AS UserId, username AS Username, email, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes FROM user"
        );
    }

    public User? GetUserById(int userId)
    {
        using var connection = CreateConnection();
        return connection.QuerySingleOrDefault<User>(
            @"SELECT ID_user AS UserId, username AS Username, email, password_hash AS PasswordHash, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes 
            FROM user
            WHERE ID_user = @UserId"
        , new { UserId = userId });
    }

    public User? GetUserByUsername(string username)
    {
        using var connection = CreateConnection();
        return connection.QuerySingleOrDefault<User>(
            @"SELECT ID_user AS UserId, username AS Username, email, password_hash AS PasswordHash, photo_url AS PhotoURL, is_admin AS IsAdmin, created_date AS CreatedAt, created_quizz AS CreatedQuizzes, taken_quizz AS ParticipatedQuizzes 
            FROM user
            WHERE username = @Username"
        , new {Username = username});
    }

    public void AddUser(User user)
    {
        using var connection = CreateConnection();

        var sql = @"INSERT INTO user (username, email, password_hash, photo_url, is_admin, created_date, created_quizz, taken_quizz) 
        VALUE (@Username, @Email, @PasswordHash, @PhotoURL, @IsAdmin, @CreatedAt, @CreatedQuizzes, @ParticipatedQuizzes)";

        connection.Execute(sql, user);
    }

    // FAIRE UN DELETE ON CASCADE pour supp le user du quizz etc etc sinon on ne sait pas delete !
    public void DeleteUser(int userId)
    {
        using var connection = CreateConnection();

        var sql = "DELETE FROM user WHERE ID_user = @UserId";

        var affectedRows = connection.Execute(sql, new { UserId = userId });

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException("aucun utilisateur trouvé avec cette id pour la suppression");
        }
    }
    
    public void UpdateUser(User user)
    {
        using var connection = CreateConnection();

        var sql = @"UPDATE user 
                    SET username = @Username, email = @Email
                    WHERE ID_user = @UserId";

        connection.Execute(sql, user);
    }
}