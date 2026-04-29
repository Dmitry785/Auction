using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        [Route("/")]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.SelectedPageName = "Home";
            return View();
        }
        [ActionName("About")]
        [HttpGet]
        public IActionResult Index2()
        {
            ViewBag.SelectedPageName = "About";
            return View();
        }
        [HttpGet]
        public IActionResult Contacts()
        {
            ViewBag.SelectedPageName = "Contacts";
            return View();
        }
        [HttpGet]
        [Route("/unauthorized")]
        public IActionResult UnauthorizedPage()
        {
            return View();
        }
    }
}
