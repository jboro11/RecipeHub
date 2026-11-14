namespace RecipeHub.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK na recept
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
