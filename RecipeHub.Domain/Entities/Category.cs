namespace RecipeHub.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // 1:N – jedna kategorie obsahuje více receptů
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
