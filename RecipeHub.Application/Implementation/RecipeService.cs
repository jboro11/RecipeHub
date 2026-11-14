using Microsoft.EntityFrameworkCore;
using RecipeHub.Application.Abstraction;
using RecipeHub.Domain.Entities;
using RecipeHub.Infrastructure;

namespace RecipeHub.Application.Implementation
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _db;

        public RecipeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Recipe>> GetAllAsync()
            => await _db.Recipes
                .Include(r => r.CategoryNavigation)
                .Include(r => r.Ratings)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IReadOnlyList<Recipe>> GetByCategoryAsync(string category)
            => await _db.Recipes
                .Include(r => r.CategoryNavigation)
                .Include(r => r.Ratings)
                .Where(r => r.Category == category || (r.CategoryNavigation != null && r.CategoryNavigation.Name == category))
                .AsNoTracking()
                .ToListAsync();

        public async Task<Recipe?> GetByIdAsync(int id)
            => await _db.Recipes
                .Include(r => r.CategoryNavigation)
                .Include(r => r.Ingredients)
                .Include(r => r.Comments)
                .Include(r => r.Ratings)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();
            return recipe;
        }

        public async Task UpdateAsync(Recipe recipe)
        {
            var existing = await _db.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == recipe.Id);

            if (existing is null)
            {
                return;
            }

            existing.Title = recipe.Title;
            existing.ShortDescription = recipe.ShortDescription;
            existing.Description = recipe.Description;
            existing.ImageUrl = recipe.ImageUrl;
            existing.Category = recipe.Category;
            existing.CategoryId = recipe.CategoryId;
            existing.PrepTime = recipe.PrepTime;
            existing.Difficulty = recipe.Difficulty;

            // Formulář odesílá celý seznam ingrediencí.
            _db.Ingredients.RemoveRange(existing.Ingredients);
            existing.Ingredients.Clear();
            foreach (var ingredient in recipe.Ingredients)
            {
                existing.Ingredients.Add(new RecipeHub.Domain.Models.Ingredient
                {
                    Name = ingredient.Name,
                    Quantity = ingredient.Quantity,
                    RecipeId = existing.Id
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Recipes.FindAsync(id);
            if (entity is null) return;
            _db.Recipes.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> RateAsync(int recipeId, string userId, int score)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            ArgumentOutOfRangeException.ThrowIfLessThan(score, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(score, 5);

            if (!await _db.Recipes.AnyAsync(r => r.Id == recipeId)) return false;

            var rating = await _db.Ratings.FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);
            if (rating is null)
            {
                _db.Ratings.Add(new Rating { RecipeId = recipeId, UserId = userId, Score = score });
            }
            else
            {
                rating.Score = score;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TitleExistsAsync(string title, int? excludeId = null)
        {
            var normalizedTitle = title.Trim().ToLower();
            var query = _db.Recipes.AsQueryable();

            if (excludeId is int id)
            {
                query = query.Where(r => r.Id != id);
            }

            return await query.AnyAsync(r => r.Title.ToLower() == normalizedTitle);
        }

        public async Task<bool> ExistsAsync(int id)
            => await _db.Recipes.AnyAsync(e => e.Id == id);
    }
}


