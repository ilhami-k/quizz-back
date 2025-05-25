using System;
using Infrastructure.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;

namespace Infrastructure.repositories;

public class CategoryRepository
{
    private readonly string _connectionString;

    public CategoryRepository()
    {
        _connectionString = "Server=localhost;Database=quizzv2;User=root;Password=admin;";
    }

    public IEnumerable<Category> GetAllCategories()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Query<Category>(
            "SELECT ID_category AS CategoryId, name FROM category"
        );
    }

    public string? GetCategoryById(int categoryId)
    {
        using var connection = new MySqlConnection(_connectionString);

        var sql = @"SELECT name FROM category 
        WHERE ID_category = @CategoryId";

        return connection.QuerySingleOrDefault<string>(sql, new { CategoryId = categoryId });
    }

    public void AddCategory(Category category)
    {
        using var connection = new MySqlConnection(_connectionString);

        var sql = @"INSERT INTO category (name, created_at)
        VALUES (@Name, @CreatedAt)";

        connection.Execute(sql, category);
    }

    public void DeleteCategory(int categoryId)
    {
        using var connection = new MySqlConnection(_connectionString);

        var sql = "DELETE FROM category WHERE ID_category = @CategoryId";

        connection.Execute(sql, new {CategoryId = categoryId});
    }
}
