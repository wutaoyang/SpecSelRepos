using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpecSelRepos.Controllers
{
    
    public class SpecSelResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Details()
        //{
        //    return View();
        //}

        //[Authorize(Roles = AccountController.ADMIN)]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[Authorize(Roles = AccountController.ADMIN)]
        //public IActionResult Delete()
        //{
        //    return View();
        //}

        //[Authorize(Roles = AccountController.ADMIN)]
        //public IActionResult Edit()
        //{
        //    return View();
        //}
    }
}