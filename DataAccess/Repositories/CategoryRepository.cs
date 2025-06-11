using System;
using Infrastructure.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using Microsoft.Extensions.Configuration;
using System.Data;
using Infrastructure.Repositories.Abstractions;

namespace Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly string _connectionString;

    public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Database connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new MySqlConnection(_connectionString);


    public IEnumerable<Category> GetAllCategories()
    {
        using var connection = CreateConnection();
        return connection.Query<Category>(
            "SELECT ID_category AS CategoryId, name FROM category"
        );
    }

    public string? GetCategoryById(int categoryId)
    {
        using var connection = CreateConnection();

        var sql = @"SELECT name FROM category 
        WHERE ID_category = @CategoryId";

        return connection.QuerySingleOrDefault<string>(sql, new { CategoryId = categoryId });
    }

    public void AddCategory(Category category)
    {
        using var connection = CreateConnection();

        var sql = @"INSERT INTO category (name, created_at)
        VALUES (@Name, @CreatedAt)";

        connection.Execute(sql, category);
    }

    public void DeleteCategory(int categoryId)
    {
        using var connection = CreateConnection();

        var sql = "DELETE FROM category WHERE ID_category = @CategoryId";

        connection.Execute(sql, new {CategoryId = categoryId});
    }
}
