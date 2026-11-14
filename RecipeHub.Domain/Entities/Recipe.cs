using RecipeHub.Domain.Models;

using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeHub.Domain.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        // Původní textová kategorie pro kompatibilitu se stávajícími view
        public string Category { get; set; } = string.Empty;

        // Časová náročnost (textový interval) a obtížnost (1–3 hvězdy)
        public string PrepTime { get; set; } = string.Empty;
        public int Difficulty { get; set; }

        // Nové napojení na entitu Category (1:N)
        public int? CategoryId { get; set; }
        public Category? CategoryNavigation { get; set; }

        // Vlastník receptu (Identity User Id)
        public string? OwnerId { get; set; }

        // Původní ukázkové hodnocení; zachováno kvůli existujícím databázím.
        public int Rating { get; set; }

        [NotMapped]
        public double? AverageRating => Ratings.Count == 0 ? null : Ratings.Average(r => r.Score);

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
