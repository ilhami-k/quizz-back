using System;
using System.Runtime.CompilerServices;
using DataAccess.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace DataAccess;

public class QuestionRepository
{
    private readonly string _connectionString;

    public QuestionRepository()
    {
        _connectionString = "Server=localhost;Database=quizzv2;User=root;Password=admin;";
    }

    public List<Answer> GetAnswersByQuestionId(int questionId)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = @"SELECT ID_answer AS AnswerId, answer_text AS AnswerText, is_correct AS CorrectAnswer, created_at AS CreatedAt 
                    FROM answer
                    WHERE ID_question = @QuestionId";
        return connection.Query<Answer>(sql, new { QuestionId = questionId }).ToList();
    }
}
