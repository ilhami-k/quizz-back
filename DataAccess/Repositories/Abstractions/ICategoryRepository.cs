using Infrastructure.Models;

namespace Infrastructure.Repositories.Abstractions;

public interface ICategoryRepository
{
    public IEnumerable<Category> GetAllCategories();
    public string? GetCategoryById(int categoryId);
    public void AddCategory(Category category);
    public void DeleteCategory(int categoryId);
}
