using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Models;
using ContosoUniversity.ViewComponents.Home;

namespace ContosoUniversity.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return ViewComponent(typeof(IndexViewComponent));
        }

        public async Task<ActionResult> About()
        {
            // Data operations are now handled in the view component
            // Dependency Injection is automatically setup for view components
            return ViewComponent(typeof(AboutViewComponent));
        }

        public IActionResult Privacy()
        {
            return ViewComponent(typeof(PrivacyViewComponent));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
