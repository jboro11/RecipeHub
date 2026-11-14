using RecipeHub.Domain.Entities;

namespace RecipeHub.Application.Abstraction
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
    }
}


