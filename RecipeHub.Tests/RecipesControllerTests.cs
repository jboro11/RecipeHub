using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeHub.Application.Implementation;
using RecipeHub.Application.ViewModels;
using RecipeHub.Controllers;
using RecipeHub.Domain.Entities;
using Xunit;

namespace RecipeHub.Tests;

public class RecipesControllerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OnlyTheOwnerOrAdminCanEditOrDeleteARecipe(bool isAdmin)
    {
        using var database = new TestDatabase();
        database.Context.Recipes.Add(new Recipe { OwnerId = "alice" });
        await database.Context.SaveChangesAsync();
        var recipe = Assert.Single(database.Context.Recipes);
        var controller = CreateController(database, "bob", isAdmin);

        var result = await controller.Edit(recipe.Id);
        if (isAdmin)
        {
            Assert.IsType<ViewResult>(result);
        }
        else
        {
            Assert.IsType<ForbidResult>(result);
            Assert.IsType<ForbidResult>(await controller.Edit(recipe.Id, new RecipeInputModel { Id = recipe.Id }));
            Assert.IsType<ForbidResult>(await controller.Delete(recipe.Id));
            Assert.IsType<ForbidResult>(await controller.DeleteConfirmed(recipe.Id));
            Assert.Single(database.Context.Recipes);
        }
    }

    [Fact]
    public async Task CreatingARecipeSetsTheOwnerAndResolvesTheCategory()
    {
        using var database = new TestDatabase();
        var category = new Category { Name = "Polévky" };
        database.Context.Categories.Add(category);
        await database.Context.SaveChangesAsync();
        var controller = CreateController(database, "alice");
        var input = ValidInput(category.Id);
        input.Id = 123;

        Assert.IsType<RedirectToActionResult>(await controller.Create(input));
        var recipe = Assert.Single(database.Context.Recipes);
        Assert.Equal("alice", recipe.OwnerId);
        Assert.Equal(category.Id, recipe.CategoryId);
        Assert.Equal(category.Name, recipe.Category);
        Assert.NotEqual(123, recipe.Id);
        Assert.Equal("Polévka", recipe.Title);
    }

    [Fact]
    public async Task UnknownCategoryIsAFormErrorInsteadOfAForeignKeyFailure()
    {
        using var database = new TestDatabase();
        var controller = CreateController(database, "alice");

        Assert.IsType<ViewResult>(await controller.Create(ValidInput(999)));
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(RecipeInputModel.CategoryId)));
        Assert.Empty(database.Context.Recipes);
    }

    [Fact]
    public async Task EditingACategoryUpdatesBothTheReferenceAndLegacyText()
    {
        using var database = new TestDatabase();
        var oldCategory = new Category { Name = "Polévky" };
        var newCategory = new Category { Name = "Hlavní jídlo" };
        database.Context.Categories.AddRange(oldCategory, newCategory);
        var recipe = new Recipe { OwnerId = "alice", CategoryNavigation = oldCategory, Category = oldCategory.Name };
        database.Context.Recipes.Add(recipe);
        await database.Context.SaveChangesAsync();
        var controller = CreateController(database, "alice");
        var input = ValidInput(newCategory.Id);
        input.Id = recipe.Id;

        Assert.IsType<RedirectToActionResult>(await controller.Edit(recipe.Id, input));
        Assert.Equal(newCategory.Id, recipe.CategoryId);
        Assert.Equal(newCategory.Name, recipe.Category);
    }

    [Fact]
    public async Task InvalidRatingIsRejectedBeforeSaving()
    {
        using var database = new TestDatabase();
        var controller = CreateController(database, "alice");

        Assert.IsType<BadRequestResult>(await controller.Rate(1, 6));
        Assert.Empty(database.Context.Ratings);
    }

    private static RecipesController CreateController(TestDatabase database, string userId, bool isAdmin = false)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        if (isAdmin) claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        return new RecipesController(new RecipeService(database.Context), new CategoryService(database.Context))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
                }
            }
        };
    }

    private static RecipeInputModel ValidInput(int categoryId) => new()
    {
        Title = " Polévka ",
        Description = "Vše uvaříme v hrnci.",
        CategoryId = categoryId,
        PrepTime = "30 minut",
        Ingredients = new List<IngredientInputModel>
        {
            new() { Name = "Voda", Quantity = "1 l" },
            new() { Name = "Zelenina", Quantity = "500 g" }
        }
    };
}
