using _20_AIProject_RecipeSuggestionWithOpenAI.Models;
using Microsoft.AspNetCore.Mvc;

namespace _20_AIProject_RecipeSuggestionWithOpenAI.Controllers
{
    public class RecipeController : Controller
    {
        private readonly OpenAIService _openAIService;

        public RecipeController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string ingredients)
        {
            var result = await _openAIService.GetRecipeAsync(ingredients);
            ViewBag.recipe = result;
            return View();
        }
    }
}
