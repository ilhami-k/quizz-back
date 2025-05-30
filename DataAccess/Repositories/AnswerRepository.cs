using System;
using Dapper;
using MySql.Data.MySqlClient;
using Infrastructure.Models;

namespace Infrastructure.Repositories;

public class AnswerRepository
{
    public readonly string _connectionString;

    public AnswerRepository()
    {
        _connectionString = "Server=localhost;Database=quizzv2;User=root;Password=admin;";
    }

    public void AddAnswer(Answer answer)
    {
        using var connection = new MySqlConnection(_connectionString);

        var sql = @"INSERT INTO answer (answer_text, is_correct, created_at)
        VALUES (@AnswerText, @CorrectAnswer, @CreatedAt)";

        connection.Execute(sql, answer);
    }
    public IEnumerable<Answer> GetAllAnswers()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Query<Answer>(
            "SELECT ID_answer AS AnswerId, answer_text AS AnswerText, is_correct AS CorrectAnswer, created_at AS CreatedAt FROM answer"
        );
    }
    public Answer? GetAnswerById(int answerId)
    {
        using var connection = new MySqlConnection(_connectionString);

        return connection.QuerySingleOrDefault<Answer>(@"SELECT ID_answer AS AnswerId, answer_text AS AnswerText, is_correct AS CorrectAnswer, created_at AS CreatedAt 
        FROM answer
        WHERE ID_answer = @AnswerId",
        new { AnswerId = answerId });
    }
}
