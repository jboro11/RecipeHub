using Microsoft.EntityFrameworkCore;
using RecipeHub.Application.Abstraction;
using RecipeHub.Domain.Entities;
using RecipeHub.Infrastructure;

namespace RecipeHub.Application.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;

        public CategoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync()
            => await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }
}


