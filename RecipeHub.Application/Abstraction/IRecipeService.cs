using RecipeHub.Domain.Entities;

namespace RecipeHub.Application.Abstraction
{
    public interface IRecipeService
    {
        Task<IReadOnlyList<Recipe>> GetAllAsync();
        Task<IReadOnlyList<Recipe>> GetByCategoryAsync(string category);
        Task<Recipe?> GetByIdAsync(int id);
        Task<Recipe> CreateAsync(Recipe recipe);
        Task UpdateAsync(Recipe recipe);
        Task DeleteAsync(int id);
        Task<bool> RateAsync(int recipeId, string userId, int score);
        Task<bool> ExistsAsync(int id);
        Task<bool> TitleExistsAsync(string title, int? excludeId = null);
    }
}


