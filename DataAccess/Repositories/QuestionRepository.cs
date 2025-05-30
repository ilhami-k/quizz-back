using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Infrastructure.Models;
using Infrastructure.Repositories.Abstractions;
using Microsoft.Extensions.Configuration; 
using MySql.Data.MySqlClient;

namespace Infrastructure.Repositories

{
    public class QuestionRepository : IQuestionRepository 
    {
        private readonly string _connectionString;

        public QuestionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Database connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

        public IEnumerable<Question> GetQuestionsByQuizId(int quizId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    q.ID_question AS QuestionId,
                    q.quizz_ID_quizz AS QuizId,
                    q.question_text AS QuestionText,
                    q.question_type AS QuestionType,
                    q.timer_seconds AS Timer,
                    q.created_at AS CreatedAt
                FROM question q
                WHERE q.quizz_ID_quizz = @QuizId;";
            
            var questions = connection.Query<Question>(sql, new { QuizId = quizId }).ToList();

            foreach (var question in questions)
            {
                question.Answers = GetAnswersByQuestionIdInternal(question.QuestionId, connection).ToList();
            }
            return questions;
        }

        public Question? GetQuestionById(int questionId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    ID_question AS QuestionId,
                    quizz_ID_quizz AS QuizId,
                    question_text AS QuestionText,
                    question_type AS QuestionType,
                    timer_seconds AS Timer,
                    created_at AS CreatedAt
                FROM question
                WHERE ID_question = @QuestionId;";
            
            var question = connection.QuerySingleOrDefault<Question>(sql, new { QuestionId = questionId });

            if (question != null)
            {
                question.Answers = GetAnswersByQuestionIdInternal(question.QuestionId, connection).ToList();
            }
            return question;
        }

        private IEnumerable<Answer> GetAnswersByQuestionIdInternal(int questionId, IDbConnection connection)
        {
            var sql = @"SELECT ID_answer AS AnswerId, answer_text AS AnswerText, is_correct AS CorrectAnswer, created_at AS CreatedAt 
                        FROM answer a
                        JOIN question_has_answer qha ON a.ID_answer = qha.answer_ID_answer
                        WHERE qha.question_ID_question = @QuestionId";
            return connection.Query<Answer>(sql, new { QuestionId = questionId });
        }
        
        public List<Answer> GetAnswersByQuestionId(int questionId)
        {
            using var connection = CreateConnection();
            return GetAnswersByQuestionIdInternal(questionId, connection).ToList();
        }

        public Question AddQuestion(Question question)
        {
            using var connection = CreateConnection();
            connection.Open(); 
            using var transaction = connection.BeginTransaction(); 

            try
            {
                var questionSql = @"
                    INSERT INTO question (quizz_ID_quizz, question_text, question_type, timer_seconds, created_at) 
                    VALUES (@QuizId, @QuestionText, @QuestionType, @Timer, @CreatedAt);
                    SELECT LAST_INSERT_ID();";
                var questionParams = new
                {
                    question.QuizId,
                    question.QuestionText,
                    question.QuestionType,
                    question.Timer,
                    question.CreatedAt
                };
                
                int newQuestionId = connection.ExecuteScalar<int>(questionSql, questionParams, transaction);
                question.QuestionId = newQuestionId; 

                if (question.Answers != null && question.Answers.Any())
                {
                    foreach (var answer in question.Answers)
                    {
                        var answerSql = @"
                            INSERT INTO answer (answer_text, is_correct, created_at) 
                            VALUES (@AnswerText, @CorrectAnswer, @CreatedAt);
                            SELECT LAST_INSERT_ID();";
                        
                        int newAnswerId = connection.ExecuteScalar<int>(answerSql, new { answer.AnswerText, answer.CorrectAnswer, answer.CreatedAt }, transaction);
                        answer.AnswerId = newAnswerId;

                        var linkSql = @"
                            INSERT INTO question_has_answer (question_ID_question, answer_ID_answer) 
                            VALUES (@QuestionId, @AnswerId);";
                        connection.Execute(linkSql, new { QuestionId = newQuestionId, AnswerId = newAnswerId }, transaction);
                    }
                }

                transaction.Commit(); 
                return question;
            }
            catch (Exception)
            {
                transaction.Rollback(); 
                throw; 
            }
        }
    }
}