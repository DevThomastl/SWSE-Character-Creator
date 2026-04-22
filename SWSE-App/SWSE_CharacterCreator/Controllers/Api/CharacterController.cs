using Microsoft.AspNetCore.Mvc;

namespace SWSE_CharacterCreator.Controllers
{
    // Handles character-related pages
    public class CharacterController : Controller
    {
        // Opens the Create page (new or existing character)
        [HttpGet]

        // Optional id lets us load a saved character if provided
        [Route("Character/Create/{id?}")]
        public IActionResult Create(int? id)
        {
            // Send the id to the view
            ViewBag.CharacterId = id;

            // Show the page
            return View();
        }
    }
}