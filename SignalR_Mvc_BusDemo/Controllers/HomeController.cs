using Microsoft.AspNetCore.Mvc;

namespace SignalRDemo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Line", new { id = 1 });

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
        ViewData["Group"] = group;
        ViewData["Title"] = title;
        return View("Line");
    }

    public IActionResult Dashboard() => View();

    public IActionResult JsTest() => View();
}
