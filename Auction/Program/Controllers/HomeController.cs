using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        [Route("/")]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.SelectedPageName = "Home";
            return View();
        }
        [ActionName("About")]
        public IActionResult Index2()
        {
            ViewBag.SelectedPageName = "About";
            return View();
        }
    }
}
