using System;
using Infrastructure.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;

namespace Infrastructure.Repositories;

public class QuizRepository
{
    private readonly string _connectionString;

    public QuizRepository()
    {
        _connectionString = "Server=localhost;Database=quizzv2;User=root;Password=admin;";
    }

    public IEnumerable<Quiz> GetAllQuizzes()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Query<Quiz>(
            "SELECT ID_quizz AS QuizId, user_id_user AS UserId, category_id_category AS CategoryId, title, description, difficulty, created_at AS CreatedAt, num_users AS ParticipantsCount, num_questions AS TotalQuestions, is_visible AS IsVisible FROM quizz"
        );
    }

    public Quiz? GetQuizById(int quizId)
    {
        using var connection = new MySqlConnection (_connectionString);

        return connection.QuerySingleOrDefault<Quiz>(@"SELECT ID_quizz AS QuizId, user_id_user AS UserId, category_id_category AS CategoryId, title, description, difficulty, created_at AS CreatedAt, num_users AS ParticipantsCount, num_questions AS TotalQuestions, is_visible AS IsVisible 
        FROM quizz
        WHERE ID_quizz = @QuizId",
        new {QuizId = quizId}) ;
    }
}