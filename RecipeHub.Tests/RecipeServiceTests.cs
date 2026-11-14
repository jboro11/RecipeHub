using Microsoft.EntityFrameworkCore;
using RecipeHub.Application.Implementation;
using RecipeHub.Domain.Entities;
using RecipeHub.Domain.Models;
using Xunit;

namespace RecipeHub.Tests;

public class RecipeServiceTests
{
    [Fact]
    public async Task RatingCanBeChangedWithoutAddingAnotherVote()
    {
        using var database = new TestDatabase();
        var service = new RecipeService(database.Context);
        var recipe = await service.CreateAsync(new Recipe { Title = "Polévka" });

        Assert.True(await service.RateAsync(recipe.Id, "alice", 5));
        Assert.True(await service.RateAsync(recipe.Id, "bob", 3));
        Assert.True(await service.RateAsync(recipe.Id, "alice", 1));

        database.Context.ChangeTracker.Clear();
        var result = await service.GetByIdAsync(recipe.Id);
        Assert.NotNull(result);
        Assert.Equal(2, result.Ratings.Count);
        Assert.Equal(2d, result.AverageRating);
        Assert.Equal(1, result.Ratings.Single(r => r.UserId == "alice").Score);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task InvalidScoreIsRejectedByTheService(int score)
    {
        using var database = new TestDatabase();
        var service = new RecipeService(database.Context);
        var recipe = await service.CreateAsync(new Recipe());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.RateAsync(recipe.Id, "alice", score));
        Assert.Empty(database.Context.Ratings);
    }

    [Fact]
    public async Task RatingRequiresAUserAndAnExistingRecipe()
    {
        using var database = new TestDatabase();
        var service = new RecipeService(database.Context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.RateAsync(1, " ", 4));
        Assert.False(await service.RateAsync(999, "alice", 4));
        Assert.Empty(database.Context.Ratings);
    }

    [Fact]
    public async Task DatabaseRejectsDuplicateVotesFromTheSameUser()
    {
        using var database = new TestDatabase();
        var recipe = new Recipe();
        database.Context.Recipes.Add(recipe);
        recipe.Ratings.Add(new Rating { UserId = "alice", Score = 3 });
        await database.Context.SaveChangesAsync();
        recipe.Ratings.Add(new Rating { UserId = "alice", Score = 5 });

        await Assert.ThrowsAsync<DbUpdateException>(() => database.Context.SaveChangesAsync());
    }

    [Fact]
    public async Task RecipeListsIncludeRealVotesAndIgnoreTheLegacyRating()
    {
        using var database = new TestDatabase();
        var service = new RecipeService(database.Context);
        var recipe = await service.CreateAsync(new Recipe { Category = "Polévky", Rating = 5 });
        Assert.Null(recipe.AverageRating);
        await service.RateAsync(recipe.Id, "alice", 2);
        database.Context.ChangeTracker.Clear();

        Assert.Equal(2d, Assert.Single(await service.GetAllAsync()).AverageRating);
        Assert.Equal(2d, Assert.Single(await service.GetByCategoryAsync("Polévky")).AverageRating);
    }

    [Fact]
    public async Task EditingPreservesOwnerAndVotesAndReplacesIngredients()
    {
        using var database = new TestDatabase();
        var service = new RecipeService(database.Context);
        var recipe = await service.CreateAsync(new Recipe
        {
            OwnerId = "alice",
            Ingredients = new List<Ingredient> { new() { Name = "Původní" } }
        });
        await service.RateAsync(recipe.Id, "bob", 4);

        await service.UpdateAsync(new Recipe
        {
            Id = recipe.Id,
            OwnerId = "bob",
            Title = "Nový název",
            ShortDescription = "Upravený úvod",
            Ingredients = new List<Ingredient> { new() { Name = "Nová", Quantity = "1 kg" } }
        });
        database.Context.ChangeTracker.Clear();

        var updated = await service.GetByIdAsync(recipe.Id);
        Assert.NotNull(updated);
        Assert.Equal("alice", updated.OwnerId);
        Assert.Equal("Upravený úvod", updated.ShortDescription);
        Assert.Equal("Nová", Assert.Single(updated.Ingredients).Name);
        Assert.Equal(4d, updated.AverageRating);
        Assert.Equal(1, await database.Context.Ingredients.CountAsync());
    }

    [Fact]
    public async Task DeletingARecipeAlsoRemovesItsRelatedData()
    {
        using var database = new TestDatabase();
        var service = new RecipeService(database.Context);
        var recipe = await service.CreateAsync(new Recipe
        {
            Ingredients = new List<Ingredient> { new() { Name = "Sůl" } },
            Comments = new List<Comment> { new() { Content = "Dobré" } }
        });
        await service.RateAsync(recipe.Id, "alice", 5);

        await service.DeleteAsync(recipe.Id);

        Assert.Empty(database.Context.Recipes);
        Assert.Empty(database.Context.Ingredients);
        Assert.Empty(database.Context.Ratings);
        Assert.Empty(database.Context.Comments);
    }
}
