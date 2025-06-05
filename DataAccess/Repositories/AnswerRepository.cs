using System;
using System.Collections.Generic; 
using System.Data;             
using Dapper;
using MySql.Data.MySqlClient;
using Infrastructure.Models;
using Infrastructure.Repositories.Abstractions;
using Microsoft.Extensions.Configuration; 

namespace Infrastructure.Repositories
{
    public class AnswerRepository : IAnswerRepository 
    {
        private readonly string _connectionString;

        public AnswerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Database connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

        public void AddAnswer(Answer answer)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO answer (answer_text, is_correct, created_at)
                        VALUES (@AnswerText, @CorrectAnswer, @CreatedAt);
                        SELECT LAST_INSERT_ID();";


            int newAnswerId = connection.ExecuteScalar<int>(sql, answer);
            answer.AnswerId = newAnswerId; 
        }

        public IEnumerable<Answer> GetAllAnswers()
        {
            using var connection = CreateConnection();
            return connection.Query<Answer>(
                "SELECT ID_answer AS AnswerId, answer_text AS AnswerText, is_correct AS CorrectAnswer, created_at AS CreatedAt FROM answer"
            );
        }

        public Answer? GetAnswerById(int answerId)
        {
            using var connection = CreateConnection();
            return connection.QuerySingleOrDefault<Answer>(
                @"SELECT ID_answer AS AnswerId, answer_text AS AnswerText, is_correct AS CorrectAnswer, created_at AS CreatedAt 
                FROM answer
                WHERE ID_answer = @AnswerId",
                new { AnswerId = answerId });
        }

        public IEnumerable<Answer> GetAnswersByQuestionId(int questionId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ID_answer AS AnswerId, answer_text AS AnswerText, is_correct AS CorrectAnswer, created_at AS CreatedAt 
                        FROM answer a
                        JOIN question_has_answer qha ON a.ID_answer = qha.answer_ID_answer
                        WHERE qha.question_ID_question = @QuestionId";
            return connection.Query<Answer>(sql, new { QuestionId = questionId });
        }
    }
}