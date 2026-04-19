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
        [ActionName("Details")]
        public IActionResult Index2()
        {
            return Ok("View()");
        }
    }
}
