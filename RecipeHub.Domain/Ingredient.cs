using RecipeHub.Domain.Entities;

namespace RecipeHub.Domain.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;

        // 🔗 Foreign Key
        public int RecipeId { get; set; }
        public Entities.Recipe Recipe { get; set; } = null!;
    }
}
