using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeHub.Application.Abstraction;
using RecipeHub.Domain.Entities;
using RecipeHub.Application.ViewModels;

namespace RecipeHub.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipes;
        private readonly ICategoryService _categories;

        public RecipesController(IRecipeService recipes, ICategoryService categories)
        {
            _recipes = recipes;
            _categories = categories;
        }

        // GET: Recipes
        public async Task<IActionResult> Index(string? category)
        {
            IReadOnlyList<Recipe> list;
            
            if (!string.IsNullOrEmpty(category))
            {
                list = await _recipes.GetByCategoryAsync(category);
                ViewBag.CurrentCategory = category;
            }
            else
            {
                list = await _recipes.GetAllAsync();
                ViewBag.CurrentCategory = null;
            }
            
            return View(list);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();
            var recipe = await _recipes.GetByIdAsync(id.Value);
            if (recipe is null) return NotFound();
            return View(recipe);
        }

        // GET: Recipes/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var model = BuildDefaultInputModel();
            await BuildFormOptionsAsync(model);
            return View(model);
        }

        // POST: Recipes/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeInputModel input)
        {
            ValidateIngredients(input);
            await ValidateCategoryAsync(input);
            if (!ModelState.IsValid)
            {
                EnsureAtLeastTwoIngredientSlots(input);
                await BuildFormOptionsAsync(input);
                return View(input);
            }

            var recipe = await ToEntityAsync(input);
            recipe.Id = 0;
            recipe.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _recipes.CreateAsync(recipe);
            return RedirectToAction(nameof(Index));
        }

        // GET: Recipes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();
            var recipe = await _recipes.GetByIdAsync(id.Value);
            if (recipe is null) return NotFound();

            if (!CanManage(recipe)) return Forbid();

            var model = new RecipeInputModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                ShortDescription = recipe.ShortDescription,
                Description = recipe.Description,
                ImageUrl = string.IsNullOrWhiteSpace(recipe.ImageUrl) ? null : recipe.ImageUrl,
                CategoryId = recipe.CategoryId ?? (await _categories.GetAllAsync())
                    .FirstOrDefault(c => c.Name == recipe.Category)?.Id,
                PrepTime = recipe.PrepTime,
                Difficulty = recipe.Difficulty,
                Ingredients = recipe.Ingredients
                    .OrderBy(i => i.Id)
                    .Select(i => new IngredientInputModel { Name = i.Name, Quantity = string.IsNullOrWhiteSpace(i.Quantity) ? null : i.Quantity })
                    .ToList()
            };

            EnsureAtLeastTwoIngredientSlots(model);
            await BuildFormOptionsAsync(model);
            return View(model);
        }

        // POST: Recipes/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeInputModel input)
        {
            if (id != input.Id) return NotFound();
            var existing = await _recipes.GetByIdAsync(id);
            if (existing is null) return NotFound();
            if (!CanManage(existing)) return Forbid();
            ValidateIngredients(input);
            await ValidateCategoryAsync(input);
            if (!ModelState.IsValid)
            {
                EnsureAtLeastTwoIngredientSlots(input);
                await BuildFormOptionsAsync(input);
                return View(input);
            }

            var recipe = await ToEntityAsync(input);
            await _recipes.UpdateAsync(recipe);
            return RedirectToAction(nameof(Index));
        }

        // GET: Recipes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var recipe = await _recipes.GetByIdAsync(id.Value);
            if (recipe is null) return NotFound();
            if (!CanManage(recipe)) return Forbid();
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _recipes.GetByIdAsync(id);
            if (recipe is null) return NotFound();
            if (!CanManage(recipe)) return Forbid();
            await _recipes.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(int id, int score)
        {
            if (score < 1 || score > 5) return BadRequest();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Challenge();
            if (!await _recipes.RateAsync(id, userId, score)) return NotFound();
            TempData["StatusMessage"] = "Vaše hodnocení bylo uloženo.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private bool CanManage(Recipe recipe) =>
            User.IsInRole("Admin") || (recipe.OwnerId is not null && recipe.OwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier));

        private async Task<Recipe> ToEntityAsync(RecipeInputModel input)
        {
            var category = input.CategoryId.HasValue
                ? await _categories.GetByIdAsync(input.CategoryId.Value)
                : null;

            var ingredients = input.Ingredients
                .Where(i => !string.IsNullOrWhiteSpace(i.Name))
                .Select(i => new RecipeHub.Domain.Models.Ingredient
                {
                    Name = i.Name.Trim(),
                    Quantity = string.IsNullOrWhiteSpace(i.Quantity) ? string.Empty : i.Quantity.Trim()
                })
                .ToList();

            return new Recipe
            {
                Id = input.Id,
                Title = input.Title.Trim(),
                ShortDescription = input.ShortDescription?.Trim(),
                Description = input.Description.Trim(),
                ImageUrl = input.ImageUrl?.Trim() ?? string.Empty,
                CategoryId = input.CategoryId,
                Category = category!.Name,
                PrepTime = input.PrepTime,
                Difficulty = input.Difficulty,
                Ingredients = ingredients
            };
        }

        private RecipeInputModel BuildDefaultInputModel()
        {
            var model = new RecipeInputModel
            {
                Difficulty = 1,
                PrepTime = string.Empty
            };
            EnsureAtLeastTwoIngredientSlots(model);
            return model;
        }

        private void EnsureAtLeastTwoIngredientSlots(RecipeInputModel model)
        {
            while (model.Ingredients.Count < 2)
            {
                model.Ingredients.Add(new IngredientInputModel());
            }
        }

        private async Task BuildFormOptionsAsync(RecipeInputModel input)
        {
            ViewBag.Categories = new SelectList(await _categories.GetAllAsync(), "Id", "Name", input.CategoryId);

            var times = new List<string>();
            for (int i = 15; i <= 180; i += 15)
            {
                if (i < 60) times.Add($"{i} minut");
                else if (i % 60 == 0) times.Add($"{i / 60} hod");
                else times.Add($"{i / 60} hod {i % 60} min");
            }
            if (!string.IsNullOrWhiteSpace(input.PrepTime) && !times.Contains(input.PrepTime))
            {
                times.Add(input.PrepTime);
            }
            ViewBag.PrepTimes = new SelectList(times, input.PrepTime);

            ViewBag.Difficulties = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "★ Lehké" },
                new SelectListItem { Value = "2", Text = "★★ Střední" },
                new SelectListItem { Value = "3", Text = "★★★ Náročné" }
            };
        }

        private async Task ValidateCategoryAsync(RecipeInputModel input)
        {
            if (input.CategoryId.HasValue && await _categories.GetByIdAsync(input.CategoryId.Value) is null)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Vyberte platnou kategorii.");
            }
        }

        private void ValidateIngredients(RecipeInputModel input)
        {
            var nonEmpty = input.Ingredients.Count(i => !string.IsNullOrWhiteSpace(i.Name));
            if (nonEmpty < 2)
            {
                ModelState.AddModelError("Ingredients", "Přidejte alespoň dvě ingredience.");
            }
        }
    }
}
