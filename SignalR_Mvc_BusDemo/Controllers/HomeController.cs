using Microsoft.AspNetCore.Mvc;

namespace SignalR_Mvc_BusDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => this.RedirectToAction("Line", new { id = 1 });

        public IActionResult Line(int id)
        {
            var (group, title) = id switch
            {
                1 => ("group-a", "Line A"),
                2 => ("group-b", "Line B"),
                3 => ("group-c", "Line C"),
                4 => ("group-d", "Line D"),
                5 => ("group-e", "Line E"),
                _ => ("group-a", "Line A")
            };
            this.ViewData["Group"] = group;
            this.ViewData["Title"] = title;
            return this.View("Line");
        }

        public IActionResult Dashboard() => this.View();
    }
}
