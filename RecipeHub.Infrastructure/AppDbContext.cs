using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeHub.Domain.Entities;
using RecipeHub.Domain.Models;

namespace RecipeHub.Infrastructure
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Délky odpovídají původní Identity migraci i při spuštění EF nástrojů.
            builder.Entity<IdentityUserLogin<string>>().Property(l => l.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(l => l.ProviderKey).HasMaxLength(128);
            builder.Entity<IdentityUserToken<string>>().Property(t => t.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserToken<string>>().Property(t => t.Name).HasMaxLength(128);

            builder.Entity<Recipe>()
                .HasMany(r => r.Ingredients)
                .WithOne(i => i.Recipe)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Recipe>()
                .HasOne(r => r.CategoryNavigation)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Recipe>()
                .HasMany(r => r.Comments)
                .WithOne(c => c.Recipe)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Recipe>()
                .HasMany(r => r.Ratings)
                .WithOne(rt => rt.Recipe)
                .HasForeignKey(rt => rt.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .Property(r => r.UserId)
                .HasMaxLength(450);

            builder.Entity<Rating>()
                .HasIndex(r => new { r.RecipeId, r.UserId })
                .IsUnique()
                .HasFilter("[UserId] IS NOT NULL");
        }
    }
}
