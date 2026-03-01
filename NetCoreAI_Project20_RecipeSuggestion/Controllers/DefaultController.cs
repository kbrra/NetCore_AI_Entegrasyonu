using Microsoft.AspNetCore.Mvc;
using NetCoreAI_Project20_RecipeSuggestion.Models;

namespace NetCoreAI_Project20_RecipeSuggestion.Controllers
{
    public class DefaultController : Controller
    {
        private readonly GroqService _groqService;

        public DefaultController()
        {
            _groqService = new GroqService();
        }

        public IActionResult CreateRecipe()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(string ingrediends)
        {
            var result = await _groqService.GetRecipeAsync(ingrediends);
            ViewBag.recipe = result;
            return View();


        }
    }
}