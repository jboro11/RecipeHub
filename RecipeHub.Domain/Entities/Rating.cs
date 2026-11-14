namespace RecipeHub.Domain.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public int Score { get; set; }

        // Starší záznamy mohou být bez uživatele; nové hlasy vyžadují přihlášení.
        public string? UserId { get; set; }

        // FK na recept
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
