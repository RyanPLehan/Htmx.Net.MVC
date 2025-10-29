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
        // See: Conventional vs Attribute Routinng (https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0)
        [HttpGet]
        [Route("")]
        [Route("index")]
        [Route("[controller]")]
        [Route("[controller]/index")]
        public IActionResult Index()
        {
            return ViewComponent(typeof(IndexViewComponent));
        }

        [HttpGet]
        [Route("[controller]/about")]
        public async Task<ActionResult> About()
        {
            // Data operations are now handled in the view component
            // Dependency Injection is automatically setup for view components
            return ViewComponent(typeof(AboutViewComponent));
        }

        [HttpGet]
        [Route("[controller]/privacy")]
        public IActionResult Privacy()
        {
            return ViewComponent(typeof(PrivacyViewComponent));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        [Route("[controller]/error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
