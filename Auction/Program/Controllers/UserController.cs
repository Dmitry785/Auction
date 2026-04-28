using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Program.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        [Authorize]
        [HttpGet]
        [Route("/user")]
        public IActionResult Index()
        {
            ViewBag.SelectedPageName = "Account";
            return View();
        }
        public IActionResult Items()
        {
            return View();
        }
    }
}
