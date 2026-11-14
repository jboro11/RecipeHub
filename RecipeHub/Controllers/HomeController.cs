using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipeHub.Models;
using RecipeHub.Application.Abstraction;

namespace RecipeHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecipeService _recipeService;

        public HomeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public async Task<IActionResult> Index()
        {
            // Načteme top 3 recepty s nejvyšším hodnocením pro "Výběr šéfkuchaře"
            var allRecipes = await _recipeService.GetAllAsync();
            var topRecipes = allRecipes
                .OrderByDescending(r => r.AverageRating ?? 0)
                .ThenByDescending(r => r.Ratings.Count)
                .ThenByDescending(r => r.Id)
                .Take(3)
                .ToList();
            
            return View(topRecipes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
