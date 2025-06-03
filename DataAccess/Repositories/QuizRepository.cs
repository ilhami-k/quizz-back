using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Infrastructure.Models;
using Infrastructure.Repositories.Abstractions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;


namespace Infrastructure.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly string _connectionString;
        private readonly IQuestionRepository _questionRepository; // Inject IQuestionRepository

        // Updated constructor
        public QuizRepository(IConfiguration configuration, IQuestionRepository questionRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Database connection string 'DefaultConnection' not found.");
            _questionRepository = questionRepository 
                ?? throw new ArgumentNullException(nameof(questionRepository));
        }

        private IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

        public IEnumerable<Quiz> GetAllQuizzes()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    q.ID_quizz AS QuizId, 
                    q.user_id_user AS UserId,       
                    q.category_id_category AS CategoryId, 
                    q.title AS Title, 
                    q.description AS Description, 
                    q.difficulty AS Dificulty,      
                    q.created_at AS CreatedAt, 
                    q.num_users AS ParticipantsCount, 
                    q.num_questions AS TotalQuestions, 
                    q.is_visible AS IsVisible,
                    
                    u.ID_user AS UserId,             
                    u.username AS Username,              
                    u.email AS Email, 
                    u.photo_url AS PhotoURL, 
                    u.is_admin AS IsAdmin, 
                    u.created_date AS CreatedAt,     
                    u.created_quizz AS CreatedQuizzes, 
                    u.taken_quizz AS ParticipatedQuizzes,
                    
                    c.ID_category AS CategoryId,     
                    c.name AS Name,                  
                    c.created_at AS CreatedAt        
                FROM quizz q
                JOIN user u ON q.user_id_user = u.ID_user   
                JOIN category c ON q.category_id_category = c.ID_category;";

            var quizzes = connection.Query<Quiz, User, Category, Quiz>(
                sql,
                (quiz, user, category) =>
                {
                    quiz.Creator = user; 
                    quiz.category = category; 
                    return quiz;
                },
                splitOn: "UserId,CategoryId" 
            ).ToList(); // Materialize the list to iterate

            if (quizzes.Any())
            {
                foreach (var quiz in quizzes)
                {
                    // Fetch and assign questions for each quiz
                    quiz.Questions = _questionRepository.GetQuestionsByQuizId(quiz.QuizId).ToList();
                }
            }
            return quizzes;
        }

        public Quiz? GetQuizById(int quizId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    q.ID_quizz AS QuizId, 
                    q.user_id_user AS UserId, 
                    q.category_id_category AS CategoryId, 
                    q.title AS Title, 
                    q.description AS Description, 
                    q.difficulty AS Dificulty, 
                    q.created_at AS CreatedAt, 
                    q.num_users AS ParticipantsCount, 
                    q.num_questions AS TotalQuestions, 
                    q.is_visible AS IsVisible,

                    u.ID_user AS UserId, 
                    u.username AS Username,              
                    u.email AS Email, 
                    u.photo_url AS PhotoURL, 
                    u.is_admin AS IsAdmin, 
                    u.created_date AS CreatedAt,
                    u.created_quizz AS CreatedQuizzes, 
                    u.taken_quizz AS ParticipatedQuizzes,

                    c.ID_category AS CategoryId, 
                    c.name AS Name, 
                    c.created_at AS CreatedAt
                FROM quizz q
                JOIN user u ON q.user_id_user = u.ID_user   
                JOIN category c ON q.category_id_category = c.ID_category
                WHERE q.ID_quizz = @QuizId;";
            
            // Use Query and then FirstOrDefault to handle the multiple mapping.
            var quizWithDetails = connection.Query<Quiz, User, Category, Quiz>(
                sql,
                (quiz, user, category) =>
                {
                    quiz.Creator = user; 
                    quiz.category = category;
                    return quiz;
                },
                new { QuizId = quizId },
                splitOn: "UserId,CategoryId"
            ).FirstOrDefault();

            if (quizWithDetails != null)
            {
                // Fetch and assign questions for the quiz
                quizWithDetails.Questions = _questionRepository.GetQuestionsByQuizId(quizWithDetails.QuizId).ToList();
            }
            
            return quizWithDetails;
        }

        public Quiz AddQuiz(Quiz quiz)
        {
            using var connection = CreateConnection();
            // Ensure Creator and category are not null and have valid IDs if they are expected to be existing.
            // For simplicity, assuming Creator.UserId and category.CategoryId are set correctly.
            var sql = @"
                INSERT INTO quizz (user_id_user, category_id_category, title, description, difficulty, created_at, num_users, num_questions, is_visible) 
                VALUES (@CreatorUserId, @QuizCategoryId, @Title, @Description, @Dificulty, @CreatedAt, @ParticipantsCount, @TotalQuestions, @IsVisible);
                SELECT LAST_INSERT_ID();";

            var parameters = new 
            {
                CreatorUserId = quiz.Creator.UserId, // Assuming Creator object is populated with UserId
                QuizCategoryId = quiz.category.CategoryId, // Assuming category object is populated with CategoryId
                quiz.Title,
                quiz.Description,
                quiz.Dificulty,
                quiz.CreatedAt,
                quiz.ParticipantsCount, // Added this as it was in the original table but not in AddQuiz before
                quiz.TotalQuestions,
                quiz.IsVisible
            };
            
            int newQuizId = connection.ExecuteScalar<int>(sql, parameters);
            quiz.QuizId = newQuizId;
            
            // Note: If questions should be added along with the quiz, that logic would go here,
            // potentially calling _questionRepository.AddQuestion for each question in quiz.Questions.
            // However, the current setup adds questions to an *existing* quiz via QuestionRoutes.

            return quiz; 
        }
        public Quiz? ToggleQuizVisibility(int quizId)
        {
            using var connection = CreateConnection();
            // GetQuizById will now also fetch questions, so we get the full model.
            var currentQuiz = GetQuizById(quizId); 
            if (currentQuiz == null)
            {
                return null;
            }
            bool newVisibility = !currentQuiz.IsVisible;
            var sql = @"
                UPDATE quizz 
                SET is_visible = @IsVisible
                WHERE ID_quizz = @QuizId;";
            
            int affectedRows = connection.Execute(sql, new { IsVisible = newVisibility, QuizId = quizId });

            if (affectedRows > 0)
            {
                currentQuiz.IsVisible = newVisibility; 
                return currentQuiz; 
            }
            return null; 
        }
    }
}