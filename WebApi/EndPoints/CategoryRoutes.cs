using System;
using Infrastructure.Repositories;

namespace Api.EndPoints;

public static class CategoryRoutes
{
    public static void AddCategoryRoutes(this WebApplication app)
    {
        app.MapGet("/categories", (CategoryRepository repo) =>
        {
            return repo.GetAllCategories();
        }   
        )
        .WithName("GetAllCategory");

        app.MapGet("/categories/{id}", (int id, CategoryRepository repo) =>
        {
            return repo.GetCategoryById(id);
        })
        .WithName("GetCategoryById");
    }
}
