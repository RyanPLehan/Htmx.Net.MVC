using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.ViewComponents.Home
{
    /// <summary>
    /// Display Privacy statement
    /// </summary>
    public class IndexViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
