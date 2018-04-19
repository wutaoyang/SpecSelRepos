using Microsoft.AspNetCore.Mvc;

namespace SpecSelRepos.Controllers
{
    public class SpecSelResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}