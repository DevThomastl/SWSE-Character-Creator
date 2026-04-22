using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SWSE_CharacterCreator.Models;

namespace SWSE_CharacterCreator.Controllers
{
    // handles basic site pages
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // inject logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // home page
        public IActionResult Index()
        {
            return View();
        }

        // privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // error page (no caching)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // pass request id to view
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}